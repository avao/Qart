using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class TestSession : ITestSession
    {
        private IList<TestCaseResult> _results;
        public IEnumerable<TestCaseResult> Results { get { return _results; } }

        public TestSession()
        {
            _results = new List<TestCaseResult>();
        }

        public void OnBegin(TestCase testCase)
        {
            _results.Add(new TestCaseResult(testCase));
        }

        public void OnFinish(TestCase testCase, Exception ex)
        {
            if (ex != null)
            {
                Results.Where(_ => _.TestCase.Id == testCase.Id).Single().MarkAsFailed(ex);
            }
        }

        public void Dispose()
        {
            _results = null;
        }
    }
}
