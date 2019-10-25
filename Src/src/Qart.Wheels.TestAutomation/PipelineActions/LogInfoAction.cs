using Microsoft.Extensions.Logging;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Framework;

namespace Qart.Wheels.TestAutomation.PipelineActions
{
    public class LogInfoAction : IPipelineAction
    {
        private readonly string _message;

        public LogInfoAction(string message)
        {
            _message = message;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.Logger.LogInformation(_message);
        }
    }
}
