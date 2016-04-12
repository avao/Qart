using Castle.Windsor;
using Newtonsoft.Json;
using Qart.Core.DataStore;
using Qart.Core.Text;
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
            var content = testCase.GetContent(".test");
            var processorName = content.LeftOfOptional("\n");

            Dictionary<string, dynamic> parameters = null;
            if(processorName != content)
            {
                parameters = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(content.RightOf("\n"));
            }
            return _container.Resolve<ITestCaseProcessor>(processorName.Trim(), parameters);
        }
    }
}
