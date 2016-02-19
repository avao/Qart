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
        private readonly IList<TestCaseResult> _results;
        public IEnumerable<TestCaseResult> Results { get { return _results; } }

        public TestSession(ITestSession customTestSession)
        {
            _results = new List<TestCaseResult>();
            _customTestSession = customTestSession;
        }

        public void OnBegin(TestCase testCase)
        {
            _results.Add(new TestCaseResult(testCase));

            if (_customTestSession != null)
            {
                _customTestSession.OnBegin(testCase);
            }
        }

        public void OnFinish(TestCase testCase, Exception ex)
        {
            if(testCase.Contains(".test"))
            {
                var processor = testCase.GetContent(".test");
            }

            var result = Results.Where(_ => _.TestCase.Id == testCase.Id).Single(); 
            if (ex != null)
            {
                result.MarkAsFailed(ex);
            }

            if (_customTestSession != null)
            {
                _customTestSession.OnFinish(result);
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
