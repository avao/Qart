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
        private readonly ITestCaseProcessorFactory _processorResolver;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISchedule<TestCase> _schedule;

        public CyberTester(ITestStorage testStorage, ITestCaseProcessorFactory processorResolver, ITestCaseLoggerFactory testCaseLoggerFactory, ILoggerFactory loggerFactory, ISchedule<TestCase> schedule, IEnumerable<ITestCasesPreprocessor> testCasesPreProcessor = null)
        {
            _testStorage = testStorage;
            _testCasesPreProcessors = testCasesPreProcessor ?? Enumerable.Empty<ITestCasesPreprocessor>();
            _testCaseLoggerFactory = testCaseLoggerFactory;
            _processorResolver = processorResolver;
            _loggerFactory = loggerFactory;
            _schedule = schedule;
        }

        public IEnumerable<TestCaseResult> RunTests(IEnumerable<ITestSession> customSessions, IDictionary<string, string> options)
        {
            //_logger.Debug("Looking for test cases.");
            var testCases = _testStorage.GetTestCaseIds().Select(_ => _testStorage.GetTestCase(_));
            testCases = _testCasesPreProcessors.Aggregate(testCases, (acc, p) => p.Execute(acc, options));
            using (var testSession = new TestSession(customSessions, _processorResolver, _testCaseLoggerFactory, _loggerFactory, options, _schedule))
            {
                testSession.Schedule(testCases, options.GetWorkersCount());
                return testSession.Results;
            }
        }
    }
}
