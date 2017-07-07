using Common.Logging;
using Qart.Testing.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qart.Testing.Framework
{
    internal class TestSession : IDisposable
    {
        private readonly IEnumerable<ITestSession> _customTestSessions;
        private readonly ITestCaseProcessorFactory _testCaseProcessorFactory;
        private readonly ITestCaseLoggerFactory _testCaseLoggerFactory;
        private readonly ILog _logger;
        private readonly ISchedule<TestCase> _schedule;
        private readonly IList<Task> _tasks;
        private readonly CancellationToken _cancellationToken;

        private readonly ConcurrentBag<TestCaseResult> _results;
        public IEnumerable<TestCaseResult> Results
        {
            get
            {
                foreach (var task in _tasks)
                {
                    task.Wait(_cancellationToken);
                }
                return _results;
            }
        }

        public IDictionary<string, string> Options { get; private set; }

        public TestSession(IEnumerable<ITestSession> customTestSessions, ITestCaseProcessorFactory testCaseProcessorFactory, ITestCaseLoggerFactory testCaseLoggerFactory, ILogManager logManager, IDictionary<string, string> options, ISchedule<TestCase> schedule)
        {
            _results = new ConcurrentBag<TestCaseResult>();
            _customTestSessions = customTestSessions;

            _testCaseProcessorFactory = testCaseProcessorFactory;
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _logger = logManager.GetLogger("");
            Options = new ReadOnlyDictionary<string, string>(options);
            _schedule = schedule;
            _tasks = new List<Task>();
            _cancellationToken = Task.Factory.CancellationToken;
        }

        public void Schedule(IEnumerable<TestCase> testCases, int workerCount)
        {
            if (_customTestSessions != null)
            {
                foreach (var session in _customTestSessions)
                    session.OnSessionStart(Options);
            }

            _schedule.Enqueue(testCases);
            for (int i = 0; i < workerCount; ++i)
            {
                _tasks.Add(Task.Factory.StartNew(() => WorkerAction(this, _schedule), _cancellationToken));
            }
        }


        private static void WorkerAction(TestSession testSession, ISchedule<TestCase> schedule)
        {
            TestCase testCase;
            while ((testCase = schedule.AcquireForProcessing(Thread.CurrentThread.ManagedThreadId.ToString())) != null)
            {
                testSession.OnTestCase(testCase);
                schedule.Dequeue(testCase);
            }
        }


        private void OnTestCase(TestCase testCase)
        {
            _logger.DebugFormat("Starting processing test case [{0}]", testCase.Id);
            var isMuted = testCase.Contains(".muted");
            if (isMuted)
                _logger.Debug("Test is muted.");

            TestCaseResult testResult = new TestCaseResult(testCase, isMuted);
            _results.Add(testResult);

            using (var testCaseContext = new TestCaseContext(Options, testCase, _testCaseLoggerFactory.GetLogger(testCase), new XDocumentDescriptionWriter()))
            {
                var descriptionWriter = new XDocumentDescriptionWriter();
                if (_customTestSessions != null)
                {
                    foreach (var session in _customTestSessions)
                        session.OnBegin(testCaseContext);
                }

                ITestCaseProcessor processor = null;
                try
                {
                    processor = _testCaseProcessorFactory.GetProcessor(testCase);
                    processor.Process(testCaseContext);
                }
                catch (Exception ex)
                {
                    testCaseContext.Logger.Error("an error occured", ex);
                    testResult.MarkAsFailed(ex);
                }
                finally
                {
                    if (processor != null)
                    {
                        _testCaseProcessorFactory.Release(processor);
                    }
                }

                try
                {
                    testResult.Description = testCaseContext.DescriptionWriter.GetContent();
                }
                catch (Exception ex)
                {
                    testCaseContext.Logger.Error("an error occured while getting test case description", ex);
                }

                if (_customTestSessions != null)
                {
                    foreach (var session in _customTestSessions)
                        session.OnFinish(testResult, testCaseContext.Logger);
                }
            }

            _logger.DebugFormat("Finished processing test case [{0}]", testCase.Id);

        }

        public void Dispose()
        {
            foreach (var session in _customTestSessions)
            {
                session.Dispose();
            }
        }
    }

}
