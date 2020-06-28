using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class ToJTokenAction : IPipelineAction
    {
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public ToJTokenAction(string sourceKey = null, string targetKey = null)
        {
            _sourceKey = sourceKey;
            _targetKey = targetKey ?? sourceKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveSourceKey = testCaseContext.GetEffectiveItemKey(_sourceKey);
            var effectiveTargetKey = testCaseContext.GetEffectiveItemKey(_targetKey);

            testCaseContext.DescriptionWriter.AddNote("ToJToken", $"{effectiveSourceKey} => {effectiveTargetKey}");
            string content = testCaseContext.GetRequiredItem<string>(effectiveSourceKey);
            testCaseContext.SetItem(effectiveTargetKey, JToken.Parse(content));
        }
    }
}
