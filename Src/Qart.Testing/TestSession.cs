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

        private readonly IList<TestCaseResult> _results;
        public IEnumerable<TestCaseResult> Results { get { return _results; } }

        public TestSession(ITestSession customTestSession, ITestCaseProcessorResolver resolver)
        {
            _results = new List<TestCaseResult>();
            _customTestSession = customTestSession;
            _testCaseProcessorResolver = resolver;
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
                processor.Process(testCase);
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
