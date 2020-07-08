using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonEditAction : IPipelineAction
    {
        private readonly string _jsonPath;
        private readonly string _value;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonEditAction(string jsonPath, string value, string sourceKey = null, string targetKey = null)
        {
            _jsonPath = jsonPath;
            _value = value;
            _sourceKey = sourceKey;
            _targetKey = targetKey ?? sourceKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveSourceKey = testCaseContext.GetEffectiveItemKey(_sourceKey);
            var effectiveTargetKey = testCaseContext.GetEffectiveItemKey(_targetKey);

            var value = testCaseContext.Resolve(_value);
            var jsonPath = testCaseContext.Resolve(_jsonPath);

            testCaseContext.DescriptionWriter.AddNote("JsonPathEdit", $"{effectiveSourceKey} => {effectiveTargetKey}. {jsonPath} => {value}");
            var jtoken = testCaseContext.GetRequiredItemAsJToken(effectiveSourceKey);
            if (effectiveSourceKey != effectiveTargetKey)
            {
                jtoken = jtoken.DeepClone();
            }

            foreach (var token in jtoken.SelectTokens(jsonPath))
            {
                token.Replace(JToken.Parse(value));
            }

            testCaseContext.SetItem(effectiveTargetKey, jtoken);
        }
    }
}
