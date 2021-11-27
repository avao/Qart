using Microsoft.Extensions.Logging;
using Qart.Core.Activation;
using Qart.Core.Collections;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Context;
using Qart.Testing.Execution;
using Qart.Testing.Framework.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Qart.Testing.Framework
{
    internal class TestSession : IDisposable
    {
        private readonly ITestSession _customTestSession;

        private readonly ILogger _logger;
        private readonly ICriticalSectionTokensProvider<TestCase> _criticalSectionTokensProvider;
        private readonly IList<Task> _tasks;
        private readonly CancellationToken _cancellationToken;
        private readonly IItemProvider _itemsInitialiser;
        private readonly IObjectFactory<IPipelineAction> _pipelineActionFactory;

        private readonly ConcurrentBag<TestCaseExecutionResult> _results;
        public IReadOnlyCollection<TestCaseExecutionResult> Results => _results;

        public IDictionary<string, string> Options { get; private set; }

        public TestSession(IObjectFactory<IPipelineAction> pipelineActionFactory, ITestSession customTestSession, ILoggerFactory loggerFactory, IDictionary<string, string> options, ICriticalSectionTokensProvider<TestCase> criticalSectionTokensProvider, IItemProvider itemProvider)
        {
            _results = new ConcurrentBag<TestCaseExecutionResult>();
            _customTestSession = customTestSession;

            _logger = loggerFactory.CreateLogger<TestSession>();
            Options = new ReadOnlyDictionary<string, string>(options);
            _criticalSectionTokensProvider = criticalSectionTokensProvider;
            _tasks = new List<Task>();
            _cancellationToken = Task.Factory.CancellationToken;
            _itemsInitialiser = itemProvider;
            _pipelineActionFactory = pipelineActionFactory;
        }

        public Task ExecuteAsync(IEnumerable<TestCase> testCases, int workerCount)
        {
            _logger.LogTrace("Scheduling tests");

            _customTestSession?.OnSessionStart(Options);

            var schedule = new CriticalSectionsAwareQueue<TestCase>();
            foreach (var testCase in testCases)
            {
                schedule.Enqueue(testCase, _criticalSectionTokensProvider.GetTokens(testCase));
            }

            for (int i = 0; i < workerCount; ++i)
            {
                _tasks.Add(Task.Factory.StartNew(async () => await WorkerActionAsync(this, schedule), _cancellationToken));
            }
            return Task.WhenAll(_tasks);
        }


        private static async Task WorkerActionAsync(TestSession testSession, CriticalSectionsAwareQueue<TestCase> schedule)
        {
            while (true)
            {
                int queueDepth;
                while (schedule.TryAcquireForProcessing(out var testCase, out queueDepth))
                {
                    testSession.OnTestCaseAsync(testCase).Wait();
                    schedule.Dequeue(testCase);
                }

                if (queueDepth > 0)
                {
                    await Task.Delay(100);
                }
                else
                {
                    break;
                }
            }
        }


        private async Task OnTestCaseAsync(TestCase testCase)
        {
            _logger.LogDebug("Starting processing test case [{0}]", testCase.Id);
            var isMuted = testCase.Contains(".muted");
            if (isMuted)
            {
                _logger.LogDebug("Test is muted, not executing.");
            }

            var testResult = new TestCaseExecutionResult(testCase, isMuted);
            _results.Add(testResult);

            using var testCaseContext = CreateContext(testCase, Options);
            var descriptionWriter = new XDocumentDescriptionWriter(testCaseContext.Logger);
            try
            {
                _customTestSession?.OnBegin(testCaseContext);
                await testCaseContext.ExecuteActionsAsync(_pipelineActionFactory, testCase.Actions, testCaseContext.Options.GetDeferExceptions(), _logger);
                testResult.Description = testCaseContext.DescriptionWriter.GetContent();
                _customTestSession?.OnFinish(testResult, testCaseContext.Logger);
            }
            catch (Exception ex)
            {
                testCaseContext.Logger.LogError(ex, "an error occured");
                testResult.MarkAsFailed(ex);
            }

            _logger.LogDebug("Finished processing test case [{0}]", testCase.Id);
        }

        public void Dispose()
        {
            _customTestSession?.Dispose();
        }


        private TestCaseContext CreateContext(TestCase testCase, IDictionary<string, string> options)
        {
            var writer = new StreamWriter(testCase.GetWriteStream("execution.log"));

            var testCaseLogger = new CompositeLogger(
                new CompositeLogger.LoggerInfo(_logger, false),
                new CompositeLogger.LoggerInfo(new TextWriterLogger(LogLevel.Debug, writer), true)
            );

            return new TestCaseContext(options, testCase, testCaseLogger, new XDocumentDescriptionWriter(testCaseLogger), new ItemsHolder(_itemsInitialiser));
        }
    }

}
