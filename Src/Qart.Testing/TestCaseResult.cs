using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
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
}
