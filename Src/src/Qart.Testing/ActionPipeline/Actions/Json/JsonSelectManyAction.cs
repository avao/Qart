using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonSelectManyAction : IPipelineAction
    {
        private readonly string _jsonPath;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonSelectManyAction(string jsonPath, string sourceKey = ItemKeys.Content, string targetKey = ItemKeys.Content)
        {
            _jsonPath = jsonPath;
            _sourceKey = sourceKey;
            _targetKey = targetKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("JsonPathSelect", $"{_sourceKey} => {_targetKey}");
            var jtoken = testCaseContext.GetRequiredItemAsJToken(_sourceKey);
            var result = new JArray(jtoken.SelectTokens(_jsonPath));
            testCaseContext.SetItem(_targetKey, result);
        }
    }
}
