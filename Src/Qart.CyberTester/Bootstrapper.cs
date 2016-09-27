using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Common.Logging;
using Qart.Core.DataStore;
using Qart.Testing;
using Qart.Testing.Framework;
using Qart.Testing.StreamTransformers;
using System.IO;
using Castle.Facilities.TypedFactory;
using Qart.Testing.Extensions.Windsor;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace Qart.CyberTester
{
    public class Bootstrapper
    {
        public static WindsorContainer CreateContainer(IDataStore testsDataStore)
        {
            var container = new WindsorContainer();
            var kernel = container.Kernel;

            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel));
            kernel.AddFacility<TypedFactoryFacility>();

            kernel.Register(Component.For<ILogManager>().ImplementedBy<LogManager>());
            kernel.Register(Component.For<ITestCaseLoggerFactory>().ImplementedBy<TestCaseLoggerFactory>());
            kernel.Register(Component.For<IDataStore>().Instance(testsDataStore).Named("testsDataStore"));
            kernel.Register(Component.For<ITestSystem>().ImplementedBy<TestSystem>());
            
            kernel.Register(Component.For<ITestCaseProcessorFactory>().AsFactory( c => c.SelectedWith(new TestCaseProcessorTypedFactoryComponentSelector())));

            //content/stream transformation
            kernel.Register(Component.For<IStreamTransformerResolver>().AsFactory());
            kernel.Register(Component.For<IContentProcessor>().ImplementedBy<ContentProcessor>());
            kernel.Register(Component.For<IStreamTransformer>().ImplementedBy<ConcatStreamTransformer>().Named("concat"));
            kernel.Register(Component.For<IStreamTransformer>().ImplementedBy<RefStreamTransformer>().Named("ref"));
                        
            container.Install(FromAssembly.InDirectory(new AssemblyFilter(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))));
            return container;
        }
    }
}
