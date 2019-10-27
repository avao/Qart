using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class AssertItemAction : IPipelineAction
    {
        private readonly string _path;
        private readonly string _key;

        public AssertItemAction(string path, string key = ItemKeys.Content)
        {
            _path = path;
            _key = key;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("AssertItem", $"{_key}");
            testCaseContext.AssertContent(testCaseContext.GetRequiredItem(_key), _path);
        }
    }
}
