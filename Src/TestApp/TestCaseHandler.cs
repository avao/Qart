using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
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
    public class ActionContext
    {
    }

    public class AnAction<T> : IPipelineAction<T>
    {
        private readonly string _somethingToSay;

        public AnAction(string somethingToSay)
        {
            _somethingToSay = somethingToSay;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            testCaseContext.Logger.Info(_somethingToSay);
        }
    }

    public class TypedFactoryComponentSelectorWithDynamicBinding : DefaultTypedFactoryComponentSelector
    {
        protected override System.Collections.IDictionary GetArguments(System.Reflection.MethodInfo method, object[] arguments)
        {
            if(arguments.Length == 1)
            {
                var dictionary = arguments[0] as System.Collections.IDictionary;
                if (dictionary != null)
                    return dictionary;
            }
            return base.GetArguments(method, arguments);
        }

    }

    public class TestCaseHandlerWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            var kernel = container.Kernel;
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel));
            kernel.AddFacility<TypedFactoryFacility>();

            kernel.Register(Component.For<ITypedFactoryComponentSelector>().ImplementedBy<TypedFactoryComponentSelectorWithDynamicBinding>());
            kernel.Register(Component.For<IPipelineActionFactory<ActionContext>>().AsFactory(c => c.SelectedWith(new TypedFactoryComponentSelectorWithDynamicBinding())));

            kernel.Register(Component.For<ActionContext>().ImplementedBy<ActionContext>());
            kernel.Register(Component.For<ITestCaseProcessor>().ImplementedBy<TestCaseHandler>().Named("blah"));
            kernel.Register(Component.For<ITestCaseProcessor>().ImplementedBy<ActionPipelineProcessor<ActionContext>>().Named("piper"));
            kernel.Register(Component.For<IPipelineAction<ActionContext>>().ImplementedBy<AnAction<ActionContext>>().Named("anAction"));
            //kernel.Register(Component.For<ITestSession>().ImplementedBy<CustomTestSession>().IsDefault());
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
