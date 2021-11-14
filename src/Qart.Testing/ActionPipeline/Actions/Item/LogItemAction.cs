using Microsoft.Extensions.Logging;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Context;
using Qart.Testing.Framework;

namespace Qart.Wheels.TestAutomation.PipelineActions
{
    public class LogItemAction : SyncActionBase
    {
        private readonly string _key;

        public LogItemAction(string key)
        {
            _key = key;
        }

        public override void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            var item = testCaseContext.GetRequiredItem(effectiveItemKey);
            testCaseContext.Logger.LogInformation(item);
        }
    }
}
