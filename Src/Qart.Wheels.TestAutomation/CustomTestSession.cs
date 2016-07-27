using System;
using System.Xml.Linq;
using Common.Logging;
using Qart.Testing;

namespace Qart.Wheels.TestAutomation
{
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
