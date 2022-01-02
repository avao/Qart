using Microsoft.Extensions.Logging;
using Qart.Core.Activation;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Context;
using Qart.Testing.Execution;
using Qart.Testing.Framework;
using Qart.Testing.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class CyberTester
    {
        private readonly ITestStorage _testStorage;
        private readonly IItemProvider _itemProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ICriticalSectionTokensProvider<TestCase> _criticalSectionTokensProvider;
        private readonly IObjectFactory<IPipelineAction> _pipelineActionFactory;
        private readonly IPriorityProvider _priorityProvider;

        public CyberTester(IObjectFactory<IPipelineAction> pipelineActionFactory, ITestStorage testStorage, ILoggerFactory loggerFactory, ICriticalSectionTokensProvider<TestCase> criticalSectionTokensProvider = null, IItemProvider itemProvider = null, IPriorityProvider priorityProvider = null)
        {
            _pipelineActionFactory = pipelineActionFactory;
            _testStorage = testStorage;
            _itemProvider = itemProvider;
            _loggerFactory = loggerFactory;
            _criticalSectionTokensProvider = criticalSectionTokensProvider;
            _priorityProvider = priorityProvider;
        }

        public async Task<IReadOnlyCollection<TestCaseExecutionResult>> RunTestsAsync(ITestSession customSession, IDictionary<string, string> options)
        {
            //discover
            var testCases = _testStorage.GetTestCaseIds().Select(id => _testStorage.GetTestCase(id));

            //filter
            testCases = FilterByTags(testCases, options);

            //order
            if (_priorityProvider != null)
            {
                testCases = testCases.OrderBy(_priorityProvider.GetPriority);
            }

            //execute
            using (var testSession = new TestSession(_pipelineActionFactory, customSession, _loggerFactory, options, _itemProvider, _criticalSectionTokensProvider))
            {
                await testSession.ExecuteAsync(testCases, options.GetWorkersCount());
                return testSession.Results;
            }
        }

        private static IEnumerable<TestCase> FilterByTags(IEnumerable<TestCase> testCases, IDictionary<string, string> options)
        {
            var includeTags = new HashSet<string>(options.GetIncludeTags(), StringComparer.InvariantCultureIgnoreCase);
            var excludeTags = new HashSet<string>(options.GetExcludeTags(), StringComparer.InvariantCultureIgnoreCase);

            return includeTags.Count > 0 || excludeTags.Count > 0
                ? testCases.Where(testCase => ShouldProcess(testCase, includeTags, excludeTags))
                : testCases;
        }

        private static bool ShouldProcess(TestCase testCase, ISet<string> includeTags, ISet<string> excludeTags)
        {
            return !includeTags.Any()
                ? testCase.Tags.All(t => !excludeTags.Contains(t))
                : testCase.Tags.Any(t => includeTags.Contains(t) && !excludeTags.Contains(t));
        }
    }
}
