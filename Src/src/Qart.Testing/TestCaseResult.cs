using System;
using System.Xml.Linq;

namespace Qart.Testing
{
    public class TestCaseResult
    {
        public TestCase TestCase { get; private set; }
        public Exception Exception { get; private set; }
        public XDocument Description { get; set; }
        public bool IsMuted { get; private set; }

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
