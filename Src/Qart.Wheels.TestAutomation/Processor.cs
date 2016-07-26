using System;
using System.Xml.Linq;
using Common.Logging;
using Castle.MicroKernel.Registration;
using Qart.Testing;

namespace Qart.Wheels.TestAutomation
{
    public class WindsorContainerInstabler : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, 
            Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Component.For<ITestCaseProcessor>().ImplementedBy<Processor>().Named("Processor"));
        }
    }

    public class Processor : ITestCaseProcessor
    {
        public void Process(TestSession testSession, TestCase testCase, ILog log)
        {
            log.InfoFormat("About to throw from {0}", testCase.Id);
            throw new NotImplementedException();
        }

        public XDocument GetDescription(TestCase testCase)
        {
            return new XDocument(new XElement("Test"));
        }
    }

    public class CustomTestSession : ITestSession
    {
        private readonly ILog _logger;
        public CustomTestSession(ILogManager logManager)
        {
            _logger = logManager.GetLogger<CustomTestSession>();
            _logger.InfoFormat("Ctor");
        }

        public void OnBegin(TestCase testCase, ILog logger)
        {
            logger.InfoFormat("OnBegin {0}", testCase.Id);
        }

        public void OnFinish(TestCaseResult result, ILog logger)
        {
            logger.InfoFormat("OnFinish {0}", result.TestCase.Id);
        }

        public void Dispose()
        {
            _logger.Info("Dispose");
        }
    }
}
