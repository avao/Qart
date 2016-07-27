using Castle.MicroKernel.Registration;
using Qart.Testing;
using Qart.Wheels.TestAutomation.TestCaseProcessors;

namespace Qart.Wheels.TestAutomation
{
    public class WindsorContainerInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container,
            Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Component.For<ITestCaseProcessor>().ImplementedBy<ThrowAnExceptionProcessor>().Named("Processor"));
        }
    }
}
