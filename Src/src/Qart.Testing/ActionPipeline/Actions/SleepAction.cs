using Qart.Testing.Framework;
using System.Threading;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class SleepAction : IPipelineAction
    {
        private readonly int _durationMs;

        public SleepAction(int durationMs)
        {
            _durationMs = durationMs;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("SleepAction", $"{_durationMs} ms");
            Thread.Sleep(_durationMs);
        }
    }
}
