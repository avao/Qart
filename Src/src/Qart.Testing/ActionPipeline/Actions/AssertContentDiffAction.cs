using Newtonsoft.Json.Linq;
using Qart.Testing.Diff;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertContentDiffAction : IPipelineAction
    {
        private readonly string _baseFile;
        private readonly string _jsonPath;
        private readonly string _itemKey;
        private readonly string _diffName;
        private readonly string _categories;
        private readonly ITokenSelectorProvider _tokenSelectorProvider;

        public AssertContentDiffAction(ITokenSelectorProvider tokenSelectorProvider, string baseFile, string diffName, string jsonPath = null, string itemKey = ItemKeys.Content, string categories = null)
        {
            _baseFile = baseFile;
            _jsonPath = jsonPath;
            _itemKey = itemKey;
            _categories = categories;
            _diffName = diffName;
            _tokenSelectorProvider = tokenSelectorProvider;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var token = testCaseContext.GetRequiredItemAsJToken(_itemKey);
            if (!string.IsNullOrEmpty(_jsonPath))
            {
                token = new JArray(token.SelectTokens(_jsonPath));
            }
            testCaseContext.TestCase.AssertContentAsDiff(token, _baseFile, _diffName, _tokenSelectorProvider, testCaseContext.Options.IsRebaseline());
        }
    }
}
