using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Qart.Testing;
using Qart.Testing.Extensions.Windsor;
using Qart.Testing.TestCaseProcessors;
using Qart.Wheels.TestAutomation.PipelineActions;
using Qart.Wheels.TestAutomation.TestCaseProcessors;

namespace Qart.Wheels.TestAutomation
{
    public class WindsorContainerInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, IConfigurationStore store)
        {
            var kernel = container.Kernel;

            //kernel.Resolver.AddSubResolver(new CollectionResolver(kernel));
            //kernel.AddFacility<TypedFactoryFacility>();


            // Example of a processor
            kernel.Register(Component.For<ITestCaseProcessor>().ImplementedBy<ThrowAnExceptionProcessor>().Named("Processor"));

            // Custom session
            kernel.Register(Component.For<ITestSession>().ImplementedBy<CustomTestSession>().IsDefault());

            // Action pipeline registration for an ActionContextExample context
            kernel.Register(Component.For<IPipelineActionFactory<ActionContextExample>>().AsFactory(c => c.SelectedWith(new TypedFactoryComponentSelectorWithDynamicBinding())));
            kernel.Register(Component.For<ActionContextExample>().ImplementedBy<ActionContextExample>());
            kernel.Register(Component.For<IPipelineContextFactory<ActionContextExample>>().AsFactory());
            kernel.Register(Component.For<ITestCaseProcessor>().ImplementedBy<ActionPipelineProcessor<ActionContextExample>>().Named("piper"));
            kernel.Register(Component.For<IPipelineAction<ActionContextExample>>().ImplementedBy<LogLineAction<ActionContextExample>>().Named("anAction"));
        }
    }

    public class ActionContextExample
    {
    }
}
