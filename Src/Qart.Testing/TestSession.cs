using Common.Logging;
using Qart.Core.Collections;
using Qart.Testing.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class TestSession : IDisposable
    {
        private readonly IEnumerable<ITestSession> _customTestSessions;
        private readonly ITestCaseProcessorFactory _testCaseProcessorFactory;
        private readonly ITestCaseLoggerFactory _testCaseLoggerFactory;
        private readonly ICriticalSectionTokensProvider<TestCase> _csTokensProvider;
        private readonly ILog _logger;
        private readonly ISchedule<TestCase> _schedule;
        private readonly IList<Task> _tasks;
        private readonly CancellationToken _cancellationToken;
        private readonly ITestCaseFilter _testCaseFilter;

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

        public TestSession(IEnumerable<ITestSession> customTestSessions, ITestCaseProcessorFactory testCaseProcessorFactory, ITestCaseLoggerFactory testCaseLoggerFactory, ILogManager logManager, IDictionary<string, string> options, ICriticalSectionTokensProvider<TestCase> csTokensProvider, ISchedule<TestCase> schedule, ITestCaseFilter testCaseFilter)
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
            _testCaseFilter = testCaseFilter;
        }

        public void Schedule(IEnumerable<TestCase> testCases, int workerCount)
        {
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

            if (!_testCaseFilter.ShouldProcess(testCase))
            {
                _logger.Debug("Test is filtered out");
                return;
            }

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

    public static class TestSessionExtensions
    {
        public static bool IsRebaseline(this TestSession testSession)
        {
            return testSession.Options.GetOptionalValue("rebase", false, bool.Parse);
        }
    }
}
