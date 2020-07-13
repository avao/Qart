using Qart.Core.DataStore;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class LoadItemAction : IPipelineAction
    {
        private readonly string _key;
        private readonly string _path;
        private readonly bool _resolve;

        public LoadItemAction(string path, string key = null, bool resolve = true)
        {
            _key = key;
            _path = path;
            _resolve = resolve;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            testCaseContext.DescriptionWriter.AddNote("LoadItem", $"file:{_path} => {effectiveItemKey}");
            var content = testCaseContext.TestCase.GetContent(_path);

            if (_resolve)
            {
                content = testCaseContext.Resolve(content);
            }

            testCaseContext.SetItem(effectiveItemKey, content);
        }
    }
}
