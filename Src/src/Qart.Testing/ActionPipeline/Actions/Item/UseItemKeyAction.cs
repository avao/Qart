using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class UseItemKeyAction : IPipelineAction
    {
        private readonly string _key;

        public UseItemKeyAction(string key)
        {
            _key = key;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            testCaseContext.DescriptionWriter.AddNote("UseItemKey", $"{effectiveItemKey}");
            testCaseContext.SetItemKey(effectiveItemKey);
        }
    }
}
