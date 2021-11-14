using Qart.Testing.Context;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonReplaceAction : IPipelineAction
    {
        private readonly string _groupsPath;
        private readonly string _sourceKey;
        private readonly string _targetKey;


        public JsonReplaceAction(string groupsPath, string sourceKey = null, string targetKey = null)
        {
            _groupsPath = groupsPath;
            _sourceKey = sourceKey;
            _targetKey = targetKey ?? sourceKey;
        }

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var effectiveSourceKey = testCaseContext.GetEffectiveItemKey(_sourceKey);
            var effectiveTargetKey = testCaseContext.GetEffectiveItemKey(_targetKey);

            testCaseContext.DescriptionWriter.AddNote("JsonReplace", $"{effectiveSourceKey} => {effectiveTargetKey}");

            var itemAsTtoken = testCaseContext.GetRequiredItemAsJToken(effectiveSourceKey);
            if (effectiveSourceKey != effectiveTargetKey)
            {
                itemAsTtoken = itemAsTtoken.DeepClone();
            }

            var groups = await testCaseContext.TestCase.GetObjectFromJsonAsync<Dictionary<string, IReadOnlyCollection<string>>>(_groupsPath);

            testCaseContext.ReplaceTokens(itemAsTtoken, groups);

            testCaseContext.SetItem(effectiveTargetKey, itemAsTtoken);
        }
    }
}
