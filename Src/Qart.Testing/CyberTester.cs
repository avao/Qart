using Common.Logging;
using Qart.Testing.Framework;
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
        private readonly ISchedule<TestCase> _schedule;
        private readonly ITestCaseFilter _testCaseFilter;

        public CyberTester(ITestSystem testSystem, ITestCaseProcessorFactory processorResolver, ITestCaseLoggerFactory testCaseLoggerFactory, ILogManager logManager, ISchedule<TestCase> schedule, ITestCaseFilter testCaseFilter = null)
        {
            _testSystem = testSystem;
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _processorResolver = processorResolver;
            _logManager = logManager;
            _schedule = schedule;
            _testCaseFilter = testCaseFilter;
        }

        public IEnumerable<TestCaseResult> RunTests(IEnumerable<ITestSession> customSessions, IDictionary<string, string> options)
        {
            //_logger.Debug("Looking for test cases.");
            IEnumerable<TestCase> testCases = _testSystem.GetTestCaseIds().Select(_ => _testSystem.GetTestCase(_));
            if (_testCaseFilter != null)
            {
                testCases = testCases.Where(_ => _testCaseFilter.ShouldProcess(_, options));
            }

            using (var testSession = new TestSession(customSessions, _processorResolver, _testCaseLoggerFactory, _logManager, options, _schedule))
            {
                testSession.Schedule(testCases, options.GetWorkersCount());
                return testSession.Results;
            }
        }
    }
}
