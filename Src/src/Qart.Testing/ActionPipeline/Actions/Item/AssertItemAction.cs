using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class AssertItemAction : IPipelineAction
    {
        private readonly string _path;
        private readonly string _key;

        public AssertItemAction(string path, string key = null)
        {
            _path = path;
            _key = key;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            testCaseContext.DescriptionWriter.AddNote("AssertItem", $"{effectiveItemKey}");
            testCaseContext.AssertContent(testCaseContext.GetRequiredItem(effectiveItemKey), _path);
        }
    }
}
