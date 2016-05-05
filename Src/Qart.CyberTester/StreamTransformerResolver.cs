using Castle.Windsor;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    public class StreamTransformerResolver : IStreamTransformerResolver
    {
        private readonly IWindsorContainer _container;

        public StreamTransformerResolver(IWindsorContainer container)
        {
            _container = container;
        }

        public IStreamTransformer ResolveTransformer(string name)
        {
            return _container.Resolve<IStreamTransformer>(name);
        }
    }
}
