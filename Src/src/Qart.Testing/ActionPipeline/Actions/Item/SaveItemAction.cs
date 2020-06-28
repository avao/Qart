using Qart.Core.DataStore;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class SaveItemAction : IPipelineAction
    {
        private readonly string _key;
        private readonly string _path;

        public SaveItemAction(string path, string key = null)
        {
            _key = key;
            _path = path;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            testCaseContext.DescriptionWriter.AddNote("SaveItem", $"{effectiveItemKey} => file: {_path}");
            testCaseContext.TestCase.PutContent(_path, testCaseContext.GetRequiredItem(effectiveItemKey));
        }
    }
}
