using Qart.Testing.Framework;
using System.Threading;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class SleepAction<T> : IPipelineAction<T>
        where T : IPipelineContext
    {
        private readonly int _durationMs;

        public SleepAction(int durationMs)
        {
            _durationMs = durationMs;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            testCaseContext.DescriptionWriter.AddNote("SleepAction", $"{_durationMs} ms");
            Thread.Sleep(_durationMs);
        }
    }
}
