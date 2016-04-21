using Castle.Windsor;
using Newtonsoft.Json;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Qart.Testing;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    public class TestCaseProcessorResolver : ITestCaseProcessorResolver
    {
        private readonly IWindsorContainer _container;
        private readonly ITestCaseProcessorInfoExtractor _paramExtractor;

        public TestCaseProcessorResolver(IWindsorContainer container)
        {
            _container = container;
            _paramExtractor = container.Resolve<ITestCaseProcessorInfoExtractor>();
        }

        public ITestCaseProcessor Resolve(TestCase testCase)
        {
            var info = _paramExtractor.Execute(testCase);
            return _container.Resolve<ITestCaseProcessor>(info.ProcessorId, (IDictionary)info.Parameters);
        }
    }
}
