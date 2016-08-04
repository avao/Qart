using Qart.Testing;
using Qart.Testing.Framework;

namespace Qart.Wheels.TestAutomation.PipelineActions
{
    public class LogLineAction<T> : IPipelineAction<T>
    {
        private readonly string _somethingToSay;

        public LogLineAction(string somethingToSay)
        {
            _somethingToSay = somethingToSay;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            testCaseContext.Logger.Info(_somethingToSay);
        }
    }
}
