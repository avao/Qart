using Castle.Windsor;
using Qart.Testing;
using System.Collections;

namespace Qart.Testing.Extensions.Windsor
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
