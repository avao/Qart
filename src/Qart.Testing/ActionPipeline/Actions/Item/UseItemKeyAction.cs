using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class UseItemKeyAction : SyncActionBase
    {
        private readonly string _key;

        public UseItemKeyAction(string key)
        {
            _key = key;
        }

        public override void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            testCaseContext.DescriptionWriter.AddNote("UseItemKey", $"{effectiveItemKey}");
            testCaseContext.SetItemKey(effectiveItemKey);
        }
    }
}
