using Newtonsoft.Json.Linq;
using Qart.Testing.Diff;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;
using System.Linq;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertContentDiffAction : IPipelineAction
    {
        private readonly string _baseFile;
        private readonly string _jsonPath;
        private readonly string _itemKey;
        private readonly string _diffName;
        private readonly string _categoriesFile;
        private readonly ITokenSelectorProvider _tokenSelectorProvider;

        public AssertContentDiffAction(ITokenSelectorProvider tokenSelectorProvider, string baseFile, string diffName, string jsonPath = null, string itemKey = ItemKeys.Content, string categoriesFile = null)
        {
            _baseFile = baseFile;
            _jsonPath = jsonPath;
            _itemKey = itemKey;
            _categoriesFile = categoriesFile;
            _diffName = diffName;
            _tokenSelectorProvider = tokenSelectorProvider;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var actual = testCaseContext.GetRequiredItemAsJToken(_itemKey);
            if (!string.IsNullOrEmpty(_jsonPath))
            {
                actual = new JArray(actual.SelectTokens(_jsonPath));
            }

            var testCase = testCaseContext.TestCase;
            var expectedBase = testCase.GetObjectFromJson<JToken>(_baseFile);

            var diffs = JsonPatchCreator.Compare(expectedBase, actual, _tokenSelectorProvider);
            (var mismatches, var expected) = testCase.CompareAndRebase(actual, expectedBase, diffs, _diffName, _tokenSelectorProvider, testCaseContext.Options.IsRebaseline());
            if (mismatches.Count > 0)
            {
                var matchedDiffCategories = testCaseContext.GetDiffCategories(actual, expected, _categoriesFile).ToList();
                throw new AssertException("Unexpected token changes:" + string.Join("\n", mismatches.Select(d => d.JsonPath)), matchedDiffCategories);
            }
        }
    }
}
