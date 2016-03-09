using System;
using System.Collections.Generic;
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

        private readonly IList<TestCaseResult> _results;
        public IEnumerable<TestCaseResult> Results { get { return _results; } }

        public TestSession(ITestSession customTestSession, ITestCaseProcessorResolver resolver, ITestCaseLoggerFactory testCaseLoggerFactory)
        {
            _results = new List<TestCaseResult>();
            _customTestSession = customTestSession;
            _testCaseProcessorResolver = resolver;
            _testCaseLoggerFactory = testCaseLoggerFactory;
        }

        public void OnTestCase(TestCase testCase)
        {
            TestCaseResult testResult= new TestCaseResult(testCase);
            _results.Add(testResult);

            if (_customTestSession != null)
            {
                _customTestSession.OnBegin(testCase);
            }

            ITestCaseProcessor processor = null;
            try
            {
                processor = _testCaseProcessorResolver.Resolve(testCase);
                using(var logger = _testCaseLoggerFactory.GetLogger(testCase))
                {
                    try
                    {
                        processor.Process(testCase, logger);
                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                testResult.MarkAsFailed(ex);
            }

            if(processor != null)
            {
                try
                {
                    testResult.Description = processor.GetDescription(testCase);
                }
                catch(Exception ex)
                {//suppress
                }
            }

            if (_customTestSession != null)
            {
                _customTestSession.OnFinish(testResult);
            }
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
