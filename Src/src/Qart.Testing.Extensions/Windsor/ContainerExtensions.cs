using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Qart.Testing.ActionPipeline;
using System.Linq;

namespace Qart.Testing.Extensions.Windsor
{
    public static class ContainerExtensions
    {
        public static void RegisterPipelineAction<T, C>(this IKernel kernel, string name)
            where T : IPipelineAction<C>
            where C : IPipelineContext
        {
            kernel.Register(Component.For<IPipelineAction<C>>().ImplementedBy<T>().Named(name).LifeStyle.Transient);
        }

        public static void RegisterPipelineAction<T>(this IKernel kernel, string name)
            where T : IPipelineAction<IPipelineContext>
        {
            var interfaces = typeof(T).GetInterfaces().Where(@interface => @interface.IsAssignableFrom(typeof(IPipelineAction<IPipelineContext>)));
            kernel.Register(Component.For(interfaces).ImplementedBy<T>().Named(name).LifeStyle.Transient);
        }
    }
}
