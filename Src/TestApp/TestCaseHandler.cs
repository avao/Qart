using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Common.Logging;
using Qart.Testing;
using Qart.Testing.Framework;
using Qart.Testing.TestCaseProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestApp
{
    public static class WindsorContainerExtensions
    {
        public static void RegisterProcessor(this IWindsorContainer container, ITestCaseProcessor processor, string name)
        {
            container.Register(Component.For<ITestCaseProcessor>().ImplementedBy<TestCaseHandler>().Named("blah"));
        }

        public static void RegisterPiplineActionInstance<T>(this IWindsorContainer container, IPipelineAction<T> action, string name)
        {
            container.Register(Component.For<IPipelineAction<T>>().Instance(action).Named(name));
        }
    }

    public class ActionContext
    {

    }

    public class AnAction<T> : IPipelineAction<T>
    {
        public string Id { get { return "anAction"; } }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            testCaseContext.Logger.Info("Typed Action Execute");
        }
    }

    public class TestCaseHandlerWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            var kernel = container.Kernel;
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel));

            container.Register(Component.For<ActionContext>().ImplementedBy<ActionContext>());
            container.Register(Component.For<ITestCaseProcessor>().ImplementedBy<TestCaseHandler>().Named("blah"));
            container.Register(Component.For<ITestCaseProcessor>().ImplementedBy<ActionPipelineProcessor<ActionContext>>().Named("piper"));
            container.RegisterPiplineActionInstance(new AnAction<ActionContext>(), "anAction");
            //container.Register(Component.For<ITestSession>().ImplementedBy<CustomTestSession>().IsDefault());
        }
    }

    public class TestCaseHandler : ITestCaseProcessor
    {
        public TestCaseHandler()
        {

        }

        public TestCaseHandler(string abc, string[] obj)
        {

        }


        public XDocument GetDescription(TestCase testCase)
        {
            return new XDocument(new XElement("blah"));
        }

        public void Process(TestCaseContext c)
        {
            c.Logger.InfoFormat("About to throw from {0}", c.TestCase.Id);
            throw new NotImplementedException();
        }
    }

    public class CustomTestSession : ITestSession
    {
        private ILog _logger;
        public CustomTestSession(ILogManager logManager)
        {
            _logger = logManager.GetLogger<CustomTestSession>();
            _logger.InfoFormat("Ctor");
        }


        public void OnBegin(TestCase testCase, ILog logger)
        {
            logger.InfoFormat("OnBegin {0}", testCase.Id);
        }

        public void OnFinish(TestCaseResult result, ILog logger)
        {
            logger.InfoFormat("OnFinish {0}", result.TestCase.Id);
        }

        public void Dispose()
        {
            _logger.Info("Dispose");
        }
    }

}
