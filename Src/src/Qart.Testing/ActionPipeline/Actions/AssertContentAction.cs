using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertJsonContentAction<T> : IPipelineAction<T>
        where T : IHttpPipelineContext
    {
        private readonly string _fileName;
        private readonly string _jsonPath;
        private readonly string _itemKey;

        public AssertJsonContentAction(string fileName, string jsonPath = null, string itemKey = PipelineContextKeys.Content)
        {
            _fileName = fileName;
            _jsonPath = jsonPath;
            _itemKey = itemKey;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            if (string.IsNullOrEmpty(_jsonPath))
            {
                testCaseContext.AssertContentJson(_itemKey, _fileName);
            }
            else
            {
                var actual = JsonConvert.DeserializeObject<JToken>(context.GetRequiredItem<string>(_itemKey));
                testCaseContext.AssertContent(actual.SelectTokens(_jsonPath).ToIndentedJson(), _fileName);
            }
        }
    }
}
