using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Qart.Testing;
using Qart.Testing.ActionPipeline;
using Qart.Testing.ActionPipeline.Actions;
using Qart.Testing.ActionPipeline.Actions.Http;
using Qart.Testing.ActionPipeline.Actions.Item;
using Qart.Testing.ActionPipeline.Actions.Json;
using Qart.Testing.Diff;
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

            //ItemsInitialiser
            kernel.Register(Component.For<IItemProvider>().ImplementedBy<ItemProvider>().IsDefault());

            //Default processor
            kernel.Register(Component.For<ITestCaseProcessor>().ImplementedBy<UrlBasedActionPipelineProcessor>().LifeStyle.Transient);

            // Pipeline actions
            kernel.Register(Component.For<IPipelineActionFactory>().AsFactory(c => c.SelectedWith(new TypedFactoryComponentSelectorWithDynamicBinding())));
            kernel.RegisterPipelineAction<HttpGetAction>("http_get");
            kernel.RegisterPipelineAction<HttpPostAction>("http_post");
            kernel.RegisterPipelineAction<HttpDeleteAction>("http_delete");
            kernel.RegisterPipelineAction<ToJTokenAction>("to_jtoken");
            kernel.RegisterPipelineAction<JsonSelectAction>("json_select");
            kernel.RegisterPipelineAction<JsonSelectManyAction>("json_select_many");
            kernel.RegisterPipelineAction<JsonRemoveAction>("json_remove");
            kernel.RegisterPipelineAction<JsonReplaceAction>("json_replace");
            kernel.RegisterPipelineAction<LoadItemAction>("load_item");
            kernel.RegisterPipelineAction<SaveItemAction>("save_item");
            kernel.RegisterPipelineAction<SetItemAction>("set_item");
            kernel.RegisterPipelineAction<AssertItemAction>("assert_item");
            kernel.RegisterPipelineAction<LogInfoAction>("log_info");
            kernel.RegisterPipelineAction<AssertContentDiffAction>("assert_diff");

            kernel.Register(Component.For<ITokenSelectorProvider>().ImplementedBy<PropertyBasedTokenSelectorProvider>().DependsOn(Dependency.OnValue("propertyName", "id")));
            

            // Example of another processor
            kernel.Register(Component.For<ITestCaseProcessor>().ImplementedBy<ThrowAnExceptionProcessor>().Named("Processor"));
        }
    }
}
