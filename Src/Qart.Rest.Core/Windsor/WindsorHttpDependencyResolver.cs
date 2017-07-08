using Castle.MicroKernel;
using Castle.Windsor;
using Qart.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace Qart.Rest.Core.Windsor
{
    public sealed class WindsorHttpDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IWindsorContainer _container;

        public WindsorHttpDependencyResolver(IWindsorContainer container)
        {
            Require.NotNull(container, "container");

            _container = container;
        }

        public object GetService(Type t)
        {
            return _container.Kernel.HasComponent(t)
             ? _container.Resolve(t) : null;
        }

        public IEnumerable<object> GetServices(Type t)
        {
            return _container.ResolveAll(t)
            .Cast<object>().ToArray();
        }

        public IDependencyScope BeginScope()
        {
            return new WindsorDependencyScope(_container);
        }

        public void Dispose()
        {
        }
    }
}
