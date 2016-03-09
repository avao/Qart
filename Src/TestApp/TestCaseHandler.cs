using Castle.MicroKernel.Registration;
using Common.Logging;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestApp
{
    public class TestCaseHandlerWindsorInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Component.For<ITestCaseProcessor>().ImplementedBy<TestCaseHandler>().Named("blah"));
            //container.Register(Component.For<ITestSession>().ImplementedBy<CustomTestSession>().IsDefault());
        }
    }

    public class TestCaseHandler : ITestCaseProcessor
    {
        public void Process(TestCase testCase, ILog log)
        {
            log.InfoFormat("About to throw from {0}", testCase.Id);
            throw new NotImplementedException();
        }


        public XDocument GetDescription(TestCase testCase)
        {
            return new XDocument(new XElement("blah"));
        }
    }

    public class CustomTestSession : ITestSession
    {
        private ILog _logger;
        public CustomTestSession(ILogManager logManager)
        {
            _logger = logManager.GetLogger<CustomTestSession>();
            _logger.InfoFormat("Ctor");
        }


        public void OnBegin(TestCase testCase)
        {
            _logger.InfoFormat("OnBegin {0}", testCase.Id);
        }

        public void OnFinish(TestCaseResult result)
        {
            _logger.InfoFormat("OnFinish {0}", result.TestCase.Id);
        }

        public void Dispose()
        {
            _logger.Info("Dispose");
        }
    }

}
