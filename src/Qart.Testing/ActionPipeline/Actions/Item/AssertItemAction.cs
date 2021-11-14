using Qart.Testing.Context;
using Qart.Testing.Framework;
using System.Threading.Tasks;

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

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            testCaseContext.DescriptionWriter.AddNote("AssertItem", $"{effectiveItemKey}");
            await testCaseContext.AssertContentAsync(testCaseContext.GetRequiredItem(effectiveItemKey), _path);
        }
    }
}
