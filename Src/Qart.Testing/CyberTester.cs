using Common.Logging;
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
        private readonly ITestCaseProcessorResolver _processorResolver;
        private readonly ILog _logger;

        public CyberTester(TestSystem testSystem, ILogManager logManager, ITestCaseProcessorResolver processorResolver)
        {
            _logger = logManager.GetLogger("");
            _testSystem = testSystem;
            _processorResolver = processorResolver;
        }

        public IEnumerable<TestCaseResult> RunTests(ITestSession customSession)
        {
            _logger.Debug("Looking for test cases.");
            var testCases = _testSystem.GetTestCases();
            using (var testSession = new TestSession(customSession, _processorResolver))
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
