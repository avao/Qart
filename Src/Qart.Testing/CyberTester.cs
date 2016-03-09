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
        private readonly ITestCaseLoggerFactory _testCaseLoggerFactory;
        private readonly ITestCaseProcessorResolver _processorResolver;
        private readonly ILogManager _logManager;

        public CyberTester(TestSystem testSystem, ITestCaseProcessorResolver processorResolver, ITestCaseLoggerFactory testCaseLoggerFactory, ILogManager logManager)
        {
            _testSystem = testSystem;
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _processorResolver = processorResolver;
            _logManager = logManager;
        }

        public IEnumerable<TestCaseResult> RunTests(ITestSession customSession)
        {
            //_logger.Debug("Looking for test cases.");
            var testCases = _testSystem.GetTestCases();
            using (var testSession = new TestSession(customSession, _processorResolver, _testCaseLoggerFactory, _logManager))
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
