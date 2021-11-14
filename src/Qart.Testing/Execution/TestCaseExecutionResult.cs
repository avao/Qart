using Qart.Testing.Framework;
using System;
using System.Xml.Linq;

namespace Qart.Testing.Execution
{
    public class TestCaseExecutionResult
    {
        public TestCase TestCase { get; }
        public Exception Exception { get; private set; }
        public XDocument Description { get; internal set; }
        public bool IsMuted { get; }

        public TestCaseExecutionResult(TestCase testCase, bool isMuted)
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
