using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertContentJsonAction : IPipelineAction
    {
        private readonly string _fileName;
        private readonly string _jsonPath;
        private readonly string _itemKey;

        public AssertContentJsonAction(string fileName, string jsonPath = null, string itemKey = ItemKeys.Content)
        {
            _fileName = fileName;
            _jsonPath = jsonPath;
            _itemKey = itemKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var token = testCaseContext.GetRequiredItemAsJToken(_itemKey);
            if (!string.IsNullOrEmpty(_jsonPath))
            {
                token = new JArray(token.SelectTokens(_jsonPath));
            }
            testCaseContext.AssertContent(token, _fileName);
        }
    }
}
