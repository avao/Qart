using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    public class TestCaseTracer : ITestCaseTracer
    {
        public class TestCaseResult
        {
            public TestCase TestCase { get; private set; }
            public Exception Exception { get; private set; }

            public TestCaseResult(TestCase testCase)
            {
                TestCase = testCase;
            }

            public void MarkAsFailed(Exception ex)
            {
                Exception = ex;
            }
        }

        public IList<TestCaseResult> Results {get; private set;}


        public TestCaseTracer()
        {
            Results = new List<TestCaseResult>();
        }

        public void OnBegin(TestCase testCase)
        {
            Results.Add(new TestCaseResult(testCase));
        }

        public void OnFailure(TestCase testCase, Exception ex)
        {
            Results.Where(_ => _.TestCase.Id == testCase.Id).Single().MarkAsFailed(ex);
        }

        public void OnFinish(TestCase testCase)
        {
        }
    }
}
