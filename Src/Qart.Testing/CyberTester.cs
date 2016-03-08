//using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class CyberTester
    {
        private readonly TestSystem _testSystem;
        //private readonly ILog Logger;

        public CyberTester(TestSystem testSystem)
        {
            //Logger = logManager.GetLogger("");
            _testSystem = testSystem;
        }

        public IEnumerable<TestCaseResult> RunTests(ITestSession customSession, ITestCaseProcessorResolver processorResolver)
        {
            //Logger.Debug("Looking for test cases.");
            var testCases = _testSystem.GetTestCases();

            if (!testCases.Any())
            {
                var testCase = _testSystem.GetTestCase(".");
                if (testCase.Contains(".test"))
                {
                    testCases = new[] { testCase };
                }
            }

            using (var testSession = new TestSession(customSession, processorResolver))
            {
                foreach (var testCase in testCases)
                {
                    testSession.OnTestCase(testCase);
                }

                return testSession.Results;
            }
        }
    }
}
