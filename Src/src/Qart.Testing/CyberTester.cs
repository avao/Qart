using Microsoft.Extensions.Logging;
using Qart.Testing.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing
{
    public class CyberTester
    {
        private readonly ITestStorage _testStorage;
        private readonly IEnumerable<ITestCasesPreprocessor> _testCasesPreProcessors;
        private readonly ITestCaseLoggerFactory _testCaseLoggerFactory;
        private readonly ITestCaseContextFactory _testCaseContextFactory;
        private readonly ITestCaseProcessorFactory _testCaseProcessorFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISchedule<TestCase> _schedule;

        public CyberTester(ITestStorage testStorage, ITestCaseProcessorFactory testCaseProcessorFactory, ITestCaseContextFactory testCaseContextFactory, ITestCaseLoggerFactory testCaseLoggerFactory, ILoggerFactory loggerFactory, ISchedule<TestCase> schedule, IEnumerable<ITestCasesPreprocessor> testCasesPreProcessor = null)
        {
            _testStorage = testStorage;
            _testCasesPreProcessors = testCasesPreProcessor ?? Enumerable.Empty<ITestCasesPreprocessor>();
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _testCaseContextFactory = testCaseContextFactory;
            _testCaseProcessorFactory = testCaseProcessorFactory;
            _loggerFactory = loggerFactory;
            _schedule = schedule;
        }

        public IEnumerable<TestCaseResult> RunTests(IEnumerable<ITestSession> customSessions, IDictionary<string, string> options)
        {
            var testCases = _testStorage.GetTestCaseIds().Select(_ => _testStorage.GetTestCase(_));

            testCases = _testCasesPreProcessors.Aggregate(testCases, (acc, p) => p.Execute(acc, options));
            using (var testSession = new TestSession(customSessions, _testCaseProcessorFactory, _testCaseLoggerFactory, _loggerFactory, options, _schedule, _testCaseContextFactory))
            {
                testSession.Schedule(testCases, options.GetWorkersCount());
                return testSession.Results;
            }
        }
    }
}
