using Common.Logging;
using Qart.Core.Collections;
using Qart.Testing.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Qart.Testing
{
    public class TestSession : IDisposable
    {
        private readonly ITestSession _customTestSession;
        private readonly ITestCaseProcessorFactory _testCaseProcessorFactory;
        private readonly ITestCaseLoggerFactory _testCaseLoggerFactory;
        private readonly ILog _logger;

        private readonly IList<TestCaseResult> _results;
        public IEnumerable<TestCaseResult> Results { get { return _results; } }

        public IDictionary<string, string> Options { get; private set; }

        public TestSession(ITestSession customTestSession, ITestCaseProcessorFactory testCaseProcessorFactory, ITestCaseLoggerFactory testCaseLoggerFactory, ILogManager logManager, IDictionary<string, string> options)
        {
            _results = new List<TestCaseResult>();
            _customTestSession = customTestSession;
            _testCaseProcessorFactory = testCaseProcessorFactory;
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _logger = logManager.GetLogger("");
            Options = new ReadOnlyDictionary<string, string>(options);
        }

        public void OnTestCase(TestCase testCase)
        {
            _logger.DebugFormat("Starting processing test case [{0}]", testCase.Id);

            TestCaseResult testResult = new TestCaseResult(testCase);
            _results.Add(testResult);

            using (var logger = _testCaseLoggerFactory.GetLogger(testCase))
            {
                var descriptionWriter = new XDocumentDescriptionWriter();
                var testCaseContext = new TestCaseContext(this, testCase, logger, descriptionWriter);
                if (_customTestSession != null)
                {
                    _customTestSession.OnBegin(testCaseContext);
                }

                ITestCaseProcessor processor = null;
                try
                {
                    processor = _testCaseProcessorFactory.GetProcessor(testCase);
                    processor.Process(testCaseContext);
                }
                catch (Exception ex)
                {
                    logger.Error("an error occured", ex);
                    testResult.MarkAsFailed(ex);
                }
                finally
                {
                    if(processor!=null)
                    {
                        _testCaseProcessorFactory.Release(processor);
                    }
                }

                try
                {
                    testResult.Description = descriptionWriter.GetContent();
                }
                catch (Exception ex)
                {
                    logger.Error("an error occured while getting test case description", ex);
                }

                if (_customTestSession != null)
                {
                    _customTestSession.OnFinish(testResult, logger);
                }
            }
            _logger.DebugFormat("Finished processing test case [{0}]", testCase.Id);

        }

        public void Dispose()
        {
            if (_customTestSession != null)
            {
                _customTestSession.Dispose();
            }
        }
    }

    public static class TestSessionExtensions
    {
        public static bool IsRebaseline(this TestSession testSession)
        {
            return testSession.Options.GetValue("rebase", false, bool.Parse);
        }
    }
}
