using Qart.Testing.Framework;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline
{
    public abstract class SyncActionBase : IPipelineAction
    {
        public Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            Execute(testCaseContext);
            return Task.CompletedTask;
        }

        public abstract void Execute(TestCaseContext testCaseContext);
    }
}
