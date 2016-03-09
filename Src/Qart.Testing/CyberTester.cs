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
        private readonly ITestCaseLoggerFactory _loggerFactory;
        private readonly ITestCaseProcessorResolver _processorResolver;
        //private readonly ILog Logger;

        public CyberTester(TestSystem testSystem, ITestCaseProcessorResolver processorResolver, ITestCaseLoggerFactory loggerFactory)
        {
            //Logger = logManager.GetLogger("");
            _testSystem = testSystem;
            _loggerFactory = loggerFactory;
            _processorResolver = processorResolver;
        }

        public IEnumerable<TestCaseResult> RunTests(ITestSession customSession)
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

            using (var testSession = new TestSession(customSession, _processorResolver, _loggerFactory))
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
