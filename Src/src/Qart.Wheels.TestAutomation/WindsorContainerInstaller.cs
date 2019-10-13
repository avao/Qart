using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Qart.Testing;
using Qart.Testing.ActionPipeline;
using Qart.Testing.ActionPipeline.Actions;
using Qart.Testing.ActionPipeline.Actions.Http;
using Qart.Testing.ActionPipeline.Actions.Json;
using Qart.Testing.Extensions.Windsor;
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

            
            // Custom session
            kernel.Register(Component.For<ITestSession>().ImplementedBy<CustomTestSession>().IsDefault());

            // Action pipeline registration for an ActionContext context
            kernel.Register(Component.For<IPipelineActionFactory<ActionContext>>().AsFactory(c => c.SelectedWith(new TypedFactoryComponentSelectorWithDynamicBinding())));
            kernel.Register(Component.For<ActionContext>().ImplementedBy<ActionContext>().LifeStyle.Transient);
            kernel.Register(Component.For<IPipelineContextFactory<ActionContext>>().AsFactory());

            //Default processor
            kernel.Register(Component.For<ITestCaseProcessor>().ImplementedBy<UrlBasedActionPipelineProcessor<ActionContext>>().LifeStyle.Transient);

            // Pipeline actions
            kernel.RegisterPipelineAction<HttpGetAction<ActionContext>, ActionContext>("http_get");
            kernel.RegisterPipelineAction<HttpPostAction<ActionContext>, ActionContext>("http_post");
            kernel.RegisterPipelineAction<HttpDeleteAction<ActionContext>, ActionContext>("http_delete");
            kernel.RegisterPipelineAction<ToJTokenAction>("to_jtoken");
            kernel.RegisterPipelineAction<JsonSelectAction>("json_select");
            kernel.RegisterPipelineAction<JsonRemoveAction>("json_remove");
            kernel.RegisterPipelineAction<LoadItemAction>("load_item");
            kernel.RegisterPipelineAction<SaveItemAction>("save_item");
            kernel.RegisterPipelineAction<SetItemAction>("set_item");
            kernel.RegisterPipelineAction<AssertItemAction>("assert_item");
            kernel.RegisterPipelineAction<LogInfoAction>("log_info");
            //kernel.Register(Component.For<IPipelineAction<IPipelineContext>>().ImplementedBy<LogInfoAction>().Named("log_info").LifeStyle.Transient);

            // Example of another processor
            kernel.Register(Component.For<ITestCaseProcessor>().ImplementedBy<ThrowAnExceptionProcessor>().Named("Processor"));
        }
    }
}
