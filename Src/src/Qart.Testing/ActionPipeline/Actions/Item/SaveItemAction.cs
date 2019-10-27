using Qart.Core.DataStore;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class SaveItemAction : IPipelineAction
    {
        private readonly string _key;
        private readonly string _path;

        public SaveItemAction(string path, string key = ItemKeys.Content)
        {
            _key = key;
            _path = path;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("SaveItem", $"{_key} => file: {_path}");
            testCaseContext.TestCase.PutContent(_path, testCaseContext.GetRequiredItem(_key));
        }
    }
}
