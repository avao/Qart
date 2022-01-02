using Microsoft.Extensions.DependencyInjection;
using Qart.Core.Activation;
using Qart.Core.DataStore;
using Qart.Testing.ActionPipeline;
using Qart.Testing.ActionPipeline.Actions;
using Qart.Testing.ActionPipeline.Actions.Http;
using Qart.Testing.ActionPipeline.Actions.Item;
using Qart.Testing.ActionPipeline.Actions.Json;
using Qart.Testing.Diff;
using Qart.Testing.Storage;
using Qart.Testing.Transformations;
using Qart.Testing.Transformations.StreamTransformers;
using Serilog;
using Serilog.Events;
using System;

namespace Qart.CyberTester
{
    public class Bootstrapper
    {
        public static ActivationRegistry<IPipelineAction> RegisterStandardServices(IServiceCollection services, IDataStore testsDataStore, LogEventLevel logEventLevel)
        {
            Log.Logger = new LoggerConfiguration()
                  .Enrich.FromLogContext()
                  .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Scope} {Message:lj}{NewLine}{Exception}")
                  .MinimumLevel.Is(logEventLevel)
                  .CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.AddSingleton<Testing.CyberTester>();

            services.AddSingleton<ITestStorage, DataStoreBasedTestStorage>();

            //DataStores. unnamed one is the default
            services.AddSingleton<IDataStoreProvider, DataStoreProvider>();
            services.AddSingleton<IDataStore>(testsDataStore);

            //Tests selection
            services.AddSingleton<Func<IDataStore, bool>>((dataStore) => dataStore.Contains(".test"));

            //content/stream transformation //TODO
            services.AddSingleton<IContentProcessor, ContentProcessor>();

            services.AddSingleton<IObjectFactory<IPipelineAction>, ObjectFactory<IPipelineAction>>();
            services.AddSingleton<IObjectFactory<IStreamTransformer>, ObjectFactory<IStreamTransformer>>();

            var actionsRegistry = new ActivationRegistry<IPipelineAction>();
            services.AddSingleton(actionsRegistry);
            RegisterBuiltInActions(actionsRegistry);

            var transformersRegistry = new ActivationRegistry<IStreamTransformer>();
            services.AddSingleton(transformersRegistry);
            RegisterBuiltInStreamTransformers(transformersRegistry);

            services.AddSingleton<ITokenSelectorProvider>(sp => new PropertyBasedTokenSelectorProvider("id"));

            return actionsRegistry;
        }

        public static void RegisterBuiltInStreamTransformers(ActivationRegistry<IStreamTransformer> registry)
        {
            registry.Register<ConcatStreamTransformer>("concat");
            registry.Register<ConcatJsonArrayStreamTransformer>("concatArray");
            registry.Register<RefStreamTransformer>("ref");
        }

        public static void RegisterBuiltInActions(ActivationRegistry<IPipelineAction> registry)
        {
            registry.Register<HttpGetAction>("http.get");
            registry.Register<HttpPutAction>("http.put");
            registry.Register<HttpPostAction>("http.post");
            registry.Register<HttpDeleteAction>("http.delete");

            registry.Register<JsonSelectAction>("json.select");
            registry.Register<JsonSelectManyAction>("json.select_many");
            registry.Register<JsonRemoveAction>("json.remove");
            registry.Register<JsonReplaceAction>("json.replace");
            registry.Register<JsonOrderAction>("json.order");
            registry.Register<JsonEditAction>("json.edit");

            registry.Register<ToJTokenAction>("to_jtoken");
            registry.Register<LoadItemAction>("item.load");
            registry.Register<SaveItemAction>("item.save");
            registry.Register<SetItemAction>("item.set");
            registry.Register<AssertItemAction>("assert");
            registry.Register<AssertContentDiffAction>("assert_diff");
            registry.Register<AssertContentJsonAction>("assert.json");

            registry.Register<SleepAction>("sleep");
            registry.Register<ExecuteAction>("execute");

            //registry.Register<LogInfoAction>("log.info");
        }
    }
}
