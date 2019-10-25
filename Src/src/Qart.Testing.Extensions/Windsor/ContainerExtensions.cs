using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Qart.Testing.ActionPipeline;

namespace Qart.Testing.Extensions.Windsor
{
    public static class ContainerExtensions
    {
        public static void RegisterPipelineAction<T>(this IKernel kernel, string name)
            where T : IPipelineAction
        {
            kernel.Register(Component.For<IPipelineAction>().ImplementedBy<T>().Named(name).LifeStyle.Transient);
        }
    }
}
