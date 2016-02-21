using Castle.Windsor;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.CyberTester
{
    public class TestCaseProcessorResolver : ITestCaseProcessorResolver
    {
        private readonly IWindsorContainer _container;

        public TestCaseProcessorResolver(IWindsorContainer container)
        {
            _container = container;
        }

        public ITestCaseProcessor Resolve(TestCase testCase)
        {
            var processorName = testCase.GetContent(".test");
            return _container.Resolve<ITestCaseProcessor>(processorName);
        }
    }
}
