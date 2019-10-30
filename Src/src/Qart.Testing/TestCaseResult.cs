using System;
using System.Xml.Linq;

namespace Qart.Testing
{
    public class TestCaseResult
    {
        public TestCase TestCase { get; }
        public Exception Exception { get; private set; }
        public XDocument Description { get; set; }
        public bool IsMuted { get; }

        public TestCaseResult(TestCase testCase, bool isMuted)
        {
            TestCase = testCase;
            IsMuted = isMuted;
        }

        public void MarkAsFailed(Exception ex)
        {
            Exception = ex;
        }
    }
}
