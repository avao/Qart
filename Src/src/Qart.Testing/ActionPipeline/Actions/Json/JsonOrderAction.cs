using Newtonsoft.Json.Linq;
using Qart.Core.Validation;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonOrderAction : IPipelineAction
    {
        private readonly string _arrayJsonPath;
        private readonly string _orderKeyPath;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonOrderAction(string arrayJsonPath, string orderKeyPath = null, string sourceKey = null, string targetKey = null)
        {
            _arrayJsonPath = arrayJsonPath;
            _orderKeyPath = orderKeyPath ?? "$";
            _sourceKey = sourceKey;
            _targetKey = targetKey ?? sourceKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveSourceKey = testCaseContext.GetEffectiveItemKey(_sourceKey);
            var effectiveTargetKey = testCaseContext.GetEffectiveItemKey(_targetKey);

            testCaseContext.DescriptionWriter.AddNote("JsonPathOrder", $"{effectiveSourceKey} => {effectiveTargetKey}");
            var itemToken = testCaseContext.GetRequiredItemAsJToken(effectiveSourceKey);
            if (effectiveSourceKey != effectiveTargetKey)
            {
                itemToken = itemToken.DeepClone();
            }

            var arrayToken = itemToken.SelectToken(_arrayJsonPath)
                .RequireType<JArray>($"{_arrayJsonPath} does not resolve into an array");

            arrayToken.OrderItems(item => item.SelectToken(_orderKeyPath).ToString());

            testCaseContext.SetItem(effectiveTargetKey, itemToken);
        }
    }
}
