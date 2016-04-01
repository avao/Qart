using Common.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class TestSession : IDisposable
    {
        private readonly ITestSession _customTestSession;
        private readonly ITestCaseProcessorResolver _testCaseProcessorResolver;
        private readonly ITestCaseLoggerFactory _testCaseLoggerFactory;
        private readonly ILog _logger;

        private readonly IList<TestCaseResult> _results;
        public IEnumerable<TestCaseResult> Results { get { return _results; } }

        public IDictionary<string, string> Options { get; private set; }

        public TestSession(ITestSession customTestSession, ITestCaseProcessorResolver resolver, ITestCaseLoggerFactory testCaseLoggerFactory, ILogManager logManager, IDictionary<string, string> options)
        {
            _results = new List<TestCaseResult>();
            _customTestSession = customTestSession;
            _testCaseProcessorResolver = resolver;
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _logger = logManager.GetLogger("");
            Options = new ReadOnlyDictionary<string, string>(options);
        }

        public void OnTestCase(TestCase testCase)
        {
            _logger.DebugFormat("Starting processing test case [{0}]", testCase.Id);

            TestCaseResult testResult= new TestCaseResult(testCase);
            _results.Add(testResult);

            using (var logger = _testCaseLoggerFactory.GetLogger(testCase))
            {

                if (_customTestSession != null)
                {
                    _customTestSession.OnBegin(testCase, logger);
                }

                ITestCaseProcessor processor = null;
                try
                {
                    processor = _testCaseProcessorResolver.Resolve(testCase);
                    processor.Process(testCase, logger);
                }
                catch (Exception ex)
                {
                    logger.Error("an error occured", ex);
                    testResult.MarkAsFailed(ex);
                }

                if (processor != null)
                {
                    try
                    {
                        testResult.Description = processor.GetDescription(testCase);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("an error occured while getting test case description", ex);
                    }
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
}
