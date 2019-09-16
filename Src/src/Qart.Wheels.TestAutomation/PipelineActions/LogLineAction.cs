using Microsoft.Extensions.Logging;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Framework;

namespace Qart.Wheels.TestAutomation.PipelineActions
{
    public class LogLineAction<T> : IPipelineAction<T>
    {
        private readonly string _somethingToSay;
        private readonly string _somethingElseToSay;


        public LogLineAction(string somethingToSay)
        {
            _somethingToSay = somethingToSay;
        }

        public LogLineAction(string somethingToSay, string somethingElse)
        {
            _somethingToSay = somethingToSay;
            _somethingElseToSay = somethingElse;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            testCaseContext.Logger.LogInformation(_somethingToSay);

            if(!string.IsNullOrEmpty(_somethingElseToSay))
            {
                testCaseContext.Logger.LogInformation(_somethingElseToSay);
            }
        }
    }
}
