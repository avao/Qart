using Common.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing
{
    public class CyberTester
    {
        private readonly ITestSystem _testSystem;
        private readonly ITestCaseLoggerFactory _testCaseLoggerFactory;
        private readonly ITestCaseProcessorFactory _processorResolver;
        private readonly ILogManager _logManager;

        public CyberTester(ITestSystem testSystem, ITestCaseProcessorFactory processorResolver, ITestCaseLoggerFactory testCaseLoggerFactory, ILogManager logManager)
        {
            _testSystem = testSystem;
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _processorResolver = processorResolver;
            _logManager = logManager;
        }

        public IEnumerable<TestCaseResult> RunTests(ITestSession customSession, IDictionary<string, string> options)
        {
            //_logger.Debug("Looking for test cases.");
            var testCases = _testSystem.GetTestCaseIds().Select(_ => _testSystem.GetTestCase(_));
            using (var testSession = new TestSession(customSession, _processorResolver, _testCaseLoggerFactory, _logManager, options))
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
