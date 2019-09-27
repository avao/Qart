using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Qart.Core.DataStore;
using Qart.Testing;
using Qart.Testing.Extensions.Windsor;
using Qart.Testing.Framework;
using Qart.Testing.StreamTransformers;
using Qart.Testing.TestCasesPreprocessors;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Qart.CyberTester
{
    public class Bootstrapper
    {
        public static IServiceProvider CreateContainer(IDataStore testsDataStore, IServiceCollection services, LogEventLevel logEventLevel)
        {
            var container = new WindsorContainer();
            var kernel = container.Kernel;

            Log.Logger = new LoggerConfiguration()
                  .Enrich.FromLogContext()
                  .WriteTo.Console()
                  .MinimumLevel.Is(logEventLevel)
                  .CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));


            kernel.Register(Component.For<ITestCaseLoggerFactory>().ImplementedBy<TestCaseLoggerFactory>());

            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel));
            kernel.AddFacility<TypedFactoryFacility>();

            kernel.Register(Component.For<Testing.CyberTester>());

            kernel.Register(Component.For<ITestStorage>().ImplementedBy<TestStorage>());

            kernel.Register(Component.For<ISchedule<TestCase>>().ImplementedBy<Schedule<TestCase>>());
            kernel.Register(Component.For<ICriticalSectionTokensProvider<TestCase>>().ImplementedBy<SequentialCriticalSectionTokensProvider<TestCase>>());

            kernel.Register(Component.For<ITestCasesPreprocessor>().ImplementedBy<TagTestCaseFilter>());

            //DataStores. unnamed one is the default
            kernel.Register(Component.For<IDataStoreProvider>().ImplementedBy<DataStoreProvider>());
            kernel.Register(Component.For<IDataStore>().Instance(testsDataStore));

            kernel.Register(Component.For<ITestCaseProcessorFactory>().AsFactory(c => c.SelectedWith(new TestCaseProcessorTypedFactoryComponentSelector())));

            //Tests selection
            kernel.Register(Component.For<Func<IDataStore, bool>>().Instance((dataStore) => dataStore.Contains(".test")));

            //content/stream transformation
            kernel.Register(Component.For<IStreamTransformerResolver>().AsFactory(c => c.SelectedWith(new TypedFactoryComponentSelectorWithDynamicBinding())));
            kernel.Register(Component.For<IContentProcessor>().ImplementedBy<ContentProcessor>());
            kernel.Register(Component.For<IStreamTransformer>().ImplementedBy<ConcatStreamTransformer>().Named("concat"));
            kernel.Register(Component.For<IStreamTransformer>().ImplementedBy<ConcatJsonArrayStreamTransformer>().Named("concatArray"));
            kernel.Register(Component.For<IStreamTransformer>().ImplementedBy<RefStreamTransformer>().Named("ref"));

            container.Install(FromAssembly.InDirectory(new AssemblyFilter(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))));

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(container, services);

            return serviceProvider;
        }
    }
}
