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
        private readonly ITestCaseProcessorInfoExtractor _paramExtractor;

        public StreamTransformerResolver(IWindsorContainer container)
        {
            _container = container;
            _paramExtractor = container.Resolve<ITestCaseProcessorInfoExtractor>();
        }

        public IStreamTransformer ResolveTransformer(string name, object param)
        {
            return _container.Resolve<IStreamTransformer>(name, param);
        }
    }
}
