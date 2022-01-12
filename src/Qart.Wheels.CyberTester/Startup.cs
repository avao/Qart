using Microsoft.Extensions.DependencyInjection;
using Qart.Core.Activation;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Execution;
using Qart.Wheels.TestAutomation;
using Qart.Wheels.TestAutomation.PipelineActions;

namespace Qart.Wheels.CyberTester
{
    public class Startup
    {
        public static void InstallServices(IServiceCollection services)
        {
            services.AddSingleton<ITestSession, CustomTestSession>();
        }

        public static void RegisterActions(ActivationRegistry<IPipelineAction> registry)
        {
            registry.Register<HelloWorldAction>("hello_world");
        }
    }
}
