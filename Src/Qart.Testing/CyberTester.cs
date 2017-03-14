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
        private readonly ICriticalSectionTokensProvider<TestCase> _csTokensProvider;
        private readonly ISchedule<TestCase> _testCase;
        private readonly ITestCaseFilter _testCaseFilter;

        public CyberTester(ITestSystem testSystem, ITestCaseProcessorFactory processorResolver, ITestCaseLoggerFactory testCaseLoggerFactory, ILogManager logManager, ICriticalSectionTokensProvider<TestCase> csTokensProvider, ISchedule<TestCase> testCase, ITestCaseFilter testCaseFilter)
        {
            _testSystem = testSystem;
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _processorResolver = processorResolver;
            _logManager = logManager;
            _csTokensProvider = csTokensProvider;
            _testCase = testCase;
            _testCaseFilter = testCaseFilter;
        }

        public IEnumerable<TestCaseResult> RunTests(IEnumerable<ITestSession> customSessions, IDictionary<string, string> options)
        {
            //_logger.Debug("Looking for test cases.");
            IEnumerable<TestCase> testCases = _testSystem.GetTestCaseIds().Select(_ => _testSystem.GetTestCase(_));
            using (var testSession = new TestSession(customSessions, _processorResolver, _testCaseLoggerFactory, _logManager, options, _csTokensProvider, _testCase, _testCaseFilter))
            {
                testSession.Schedule(testCases, 4);//TODO get worker count from options
                return testSession.Results;
            }
        }
    }
}
