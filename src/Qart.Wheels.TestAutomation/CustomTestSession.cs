using Microsoft.Extensions.Logging;
using Qart.Testing.Execution;
using Qart.Testing.Framework;
using System.Collections.Generic;

namespace Qart.Wheels.TestAutomation
{
    public class CustomTestSession : ITestSession
    {
        private readonly ILogger _logger;
        public CustomTestSession(ILoggerFactory logManager)
        {
            _logger = logManager.CreateLogger<CustomTestSession>();
            _logger.LogInformation("Ctor");
        }

        public void OnSessionStart(IDictionary<string, string> options)
        {
        }

        public void OnBegin(TestCaseContext ctx)
        {
            ctx.Logger.LogInformation("OnBegin {0}", ctx.TestCase.Id);
        }

        public void OnFinish(TestCaseExecutionResult result, ILogger logger)
        {
            logger.LogInformation("OnFinish {0}", result.TestCase.Id);
        }

        public void Dispose()
        {
            _logger.LogInformation("Dispose");
        }
    }
}
