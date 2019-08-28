using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertJsonContentAction<T> : IPipelineAction<T>
        where T : IHttpContext
    {
        private readonly string _fileName;
        private readonly string _jsonPath;

        public AssertJsonContentAction(string fileName, string jsonPath = null)
        {
            _fileName = fileName;
            _jsonPath = jsonPath;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            if (string.IsNullOrEmpty(_jsonPath))
            {
                testCaseContext.AssertContentJson(context.Content, _fileName);
            }
            else
            {
                var actual = JsonConvert.DeserializeObject<JToken>(context.Content);
                testCaseContext.AssertContent(actual.SelectTokens(_jsonPath).ToIndentedJson(), _fileName);
            }
        }
    }
}
