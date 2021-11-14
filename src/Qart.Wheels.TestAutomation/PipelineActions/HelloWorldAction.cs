using Microsoft.Extensions.Logging;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Framework;

namespace Qart.Wheels.TestAutomation.PipelineActions
{
    public class HelloWorldAction : SyncActionBase
    {
        private readonly string _message;

        public HelloWorldAction(string message)
        {
            _message = message;
        }

        public override void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.Logger.LogInformation($"Hello world! {_message}");
        }
    }
}
