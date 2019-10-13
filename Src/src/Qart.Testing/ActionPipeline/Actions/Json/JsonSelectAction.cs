using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonSelectAction : IPipelineAction<IPipelineContext>
    {
        private readonly string _jsonPath;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonSelectAction(string jsonPath, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
        {
            _jsonPath = jsonPath;
            _sourceKey = sourceKey;
            _targetKey = targetKey;
        }

        public void Execute(TestCaseContext testCaseContext, IPipelineContext context)
        {
            testCaseContext.DescriptionWriter.AddNote("JsonPathSelect", $"{_sourceKey} => {_targetKey}");
            var jtoken = context.GetRequiredItemAsJToken(_sourceKey);
            var result = new JArray(jtoken.SelectTokens(_jsonPath));
            context.SetItem(_targetKey, result);
        }
    }
}
