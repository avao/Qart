using Qart.Core.DataStore;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class LoadItemAction : IPipelineAction
    {
        private readonly string _key;
        private readonly string _path;

        public LoadItemAction(string path, string key = ItemKeys.Content)
        {
            _key = key;
            _path = path;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("LoadItem", $"file:{_path} => {_key}");
            var content = testCaseContext.TestCase.GetContent(_path);
            testCaseContext.SetItem(_key, content);
        }
    }
}
