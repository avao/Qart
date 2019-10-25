using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertJsonContentAction : IPipelineAction
    {
        private readonly string _fileName;
        private readonly string _jsonPath;
        private readonly string _itemKey;

        public AssertJsonContentAction(string fileName, string jsonPath = null, string itemKey = ItemKeys.Content)
        {
            _fileName = fileName;
            _jsonPath = jsonPath;
            _itemKey = itemKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            string value = testCaseContext.GetRequiredItem(_itemKey);
            if (!string.IsNullOrEmpty(_jsonPath))
            {
                value = JsonConvert.DeserializeObject<JToken>(value).SelectTokens(_jsonPath).ToIndentedJson();
            }
            testCaseContext.AssertContentJson(value, _fileName);
        }
    }
}
