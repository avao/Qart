using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Common.Logging;
using Qart.Testing;
using Qart.Testing.StreamTransformers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    public class Bootstrapper
    {
        public static WindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<ILogManager>().ImplementedBy<LogManager>());
            container.Register(Component.For<ITestCaseLoggerFactory>().ImplementedBy<TestCaseLoggerFactory>());
            container.Register(Component.For<ITestCaseProcessorInfoExtractor>().ImplementedBy<TestCaseProcessorInfoExtractor>());
            
            container.Register(Component.For<ITestCaseProcessorResolver>().Instance(new TestCaseProcessorResolver(container)));
            container.Register(Component.For<IStreamTransformerResolver>().Instance(new StreamTransformerResolver(container)));
            container.Register(Component.For<IContentProcessor>().ImplementedBy<ContentProcessor>());
            container.Register(Component.For<IStreamTransformer>().ImplementedBy<ConcatStreamTransformer>().Named("concat"));
            container.Register(Component.For<IStreamTransformer>().ImplementedBy<RefStreamTransformer>().Named("ref"));
            
            container.Install(FromAssembly.InDirectory(new AssemblyFilter(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))));
            return container;
        }
    }
}
