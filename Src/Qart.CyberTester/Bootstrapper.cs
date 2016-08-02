using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Common.Logging;
using Qart.Core.DataStore;
using Qart.Testing;
using Qart.Testing.Framework;
using Qart.Testing.StreamTransformers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;

namespace Qart.CyberTester
{
    public class Bootstrapper
    {
        public static WindsorContainer CreateContainer(IDataStore testsDataStore)
        {
            var container = new WindsorContainer();
            var kernel = container.Kernel;
            kernel.Register(Component.For<ILogManager>().ImplementedBy<LogManager>());
            kernel.Register(Component.For<ITestCaseLoggerFactory>().ImplementedBy<TestCaseLoggerFactory>());
            kernel.Register(Component.For<ITestCaseProcessorInfoExtractor>().ImplementedBy<TestCaseProcessorInfoExtractor>());
            kernel.Register(Component.For<IDataStore>().Instance(testsDataStore).Named("testsDataStore"));
            kernel.Register(Component.For<ITestSystem>().ImplementedBy<TestSystem>());
            
            kernel.Register(Component.For<ITestCaseProcessorResolver>().Instance(new TestCaseProcessorResolver(container)));

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
