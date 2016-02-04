using Castle.MicroKernel.Registration;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class TestCaseHandlerWindsorInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Component.For<ITestCaseProcessor>().ImplementedBy<TestCaseHandler>().Named("blah"));
        }
    }

    public class TestCaseHandler : ITestCaseProcessor
    {

        public void Process(TestCase testCase)
        {
            throw new NotImplementedException();
        }
    }
}
