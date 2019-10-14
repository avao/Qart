using Newtonsoft.Json.Linq;
using Qart.Core.Validation;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonOrderAction : IPipelineAction<IPipelineContext>
    {
        private readonly string _arrayJsonPath;
        private readonly string _orderKeyPath;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonOrderAction(string arrayJsonPath, string orderKeyPath, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
        {
            _arrayJsonPath = arrayJsonPath;
            _orderKeyPath = orderKeyPath;
            _sourceKey = sourceKey;
            _targetKey = targetKey;
        }

        public void Execute(TestCaseContext testCaseContext, IPipelineContext context)
        {
            testCaseContext.DescriptionWriter.AddNote("JsonPathOrder", $"{_sourceKey} => {_targetKey}");
            var jtoken = context.GetRequiredItemAsJToken(_sourceKey);
            if (_sourceKey != _targetKey)
            {
                jtoken = jtoken.DeepClone();
            }

            var arrayToken = jtoken.SelectToken(_arrayJsonPath)
                .RequireType<JArray>($"{_arrayJsonPath} does not resolve into an array");

            arrayToken.OrderItems(item => item.SelectToken(_orderKeyPath).ToString());

            context.SetItem(_targetKey, jtoken);
        }
    }
}
