using System;
using System.Xml.Linq;
using Common.Logging;
using Qart.Testing;
using Qart.Testing.Framework;

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

        public void OnBegin(TestCaseContext ctx)
        {
            ctx.Logger.InfoFormat("OnBegin {0}", ctx.TestCase.Id);
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
