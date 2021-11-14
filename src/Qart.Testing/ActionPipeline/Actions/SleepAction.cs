using Qart.Testing.Framework;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class SleepAction : IPipelineAction
    {
        private readonly int _durationMs;

        public SleepAction(int durationMs)
        {
            _durationMs = durationMs;
        }

        public Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("SleepAction", $"{_durationMs} ms");
            return Task.Delay(_durationMs);
        }
    }
}
