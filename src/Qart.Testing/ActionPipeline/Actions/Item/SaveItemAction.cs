using Qart.Core.DataStore;
using Qart.Testing.Context;
using Qart.Testing.Framework;
using System.Threading.Tasks;

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

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            testCaseContext.DescriptionWriter.AddNote("SaveItem", $"{effectiveItemKey} => file: {_path}");
            await testCaseContext.TestCase.PutContentAsync(_path, testCaseContext.GetRequiredItem(effectiveItemKey));
        }
    }
}
