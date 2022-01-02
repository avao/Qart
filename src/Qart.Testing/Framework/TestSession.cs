using Microsoft.Extensions.Logging;
using Qart.Core.Activation;
using Qart.Core.Collections;
using Qart.Core.Tracing;
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

        public TestSession(IObjectFactory<IPipelineAction> pipelineActionFactory, ITestSession customTestSession, ILoggerFactory loggerFactory, IDictionary<string, string> options, IItemProvider itemProvider, ICriticalSectionTokensProvider<TestCase> criticalSectionTokensProvider = null)
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
                schedule.Enqueue(testCase, _criticalSectionTokensProvider?.GetTokens(testCase) ?? Array.Empty<string>());
            }

            for (int i = 0; i < workerCount; ++i)
            {
                _tasks.Add(WorkerActionAsync(this, schedule, _cancellationToken));
            }
            return Task.WhenAll(_tasks);
        }


        private static async Task WorkerActionAsync(TestSession testSession, CriticalSectionsAwareQueue<TestCase> schedule, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int queueDepth;
                while (schedule.TryAcquireForProcessing(out var testCase, out queueDepth))
                {
                    await testSession.ExecuteTestCaseAsync(testCase);
                    schedule.Dequeue(testCase);
                }

                if (queueDepth == 0)
                {
                    break;
                }
                await Task.Delay(100, cancellationToken);
            }
        }


        private async Task ExecuteTestCaseAsync(TestCase testCase)
        {
            var correlationId = Correlation.GetTickBasedId();
            using var scope = _logger.BeginScope(correlationId);

            _logger.LogDebug("Starting processing test case [{0}]", testCase.Id);
            var isMuted = testCase.Contains(".muted");
            if (isMuted)
            {
                _logger.LogDebug("Test is muted, not executing.");
            }

            var testResult = new TestCaseExecutionResult(testCase, isMuted);
            _results.Add(testResult);

            using var testCaseContext = CreateContext(testCase, correlationId, Options);
            var descriptionWriter = new XDocumentDescriptionWriter(testCaseContext.Logger);
            try
            {
                _customTestSession?.OnBegin(testCaseContext);
                await testCaseContext.ExecuteActionsAsync(_pipelineActionFactory, testCase.Actions, testCaseContext.Options.GetDeferExceptions(), _logger);
            }
            catch (Exception ex)
            {
                testCaseContext.Logger.LogError(ex, "an error occured");
                testResult.MarkAsFailed(ex);
            }

            try
            {
                testResult.Description = testCaseContext.DescriptionWriter.GetContent();
                _customTestSession?.OnFinish(testResult, testCaseContext.Logger);
            }
            catch (Exception ex)
            {
                testCaseContext.Logger.LogError(ex, "an error occured");
            }

            _logger.LogDebug("Finished processing test case [{0}]", testCase.Id);
        }

        public void Dispose()
        {
            _customTestSession?.Dispose();
        }


        private TestCaseContext CreateContext(TestCase testCase, string correlationId, IDictionary<string, string> options)
        {
            var writer = new StreamWriter(testCase.GetWriteStream("execution.log"));

            var testCaseLogger = new CompositeLogger(
                new CompositeLogger.LoggerInfo(_logger, false),
                new CompositeLogger.LoggerInfo(new TextWriterLogger(LogLevel.Debug, writer), true)
            );

            return new TestCaseContext(options, testCase, correlationId, testCaseLogger, new XDocumentDescriptionWriter(testCaseLogger), new ItemsHolder(_itemsInitialiser));
        }
    }
}
