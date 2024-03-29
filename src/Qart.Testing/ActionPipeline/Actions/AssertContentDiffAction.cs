﻿using Newtonsoft.Json.Linq;
using Qart.Testing.Context;
using Qart.Testing.Diff;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;
using System.Linq;
using System.Threading.Tasks;

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

        public AssertContentDiffAction(ITokenSelectorProvider tokenSelectorProvider, string baseFile, string diffName, string jsonPath = null, string itemKey = null, string categoriesFile = null)
        {
            _baseFile = baseFile;
            _jsonPath = jsonPath;
            _itemKey = itemKey;
            _categoriesFile = categoriesFile;
            _diffName = diffName;
            _tokenSelectorProvider = tokenSelectorProvider;
        }

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_itemKey);
            
            testCaseContext.DescriptionWriter.AddNote("AssertContentDiff", $"{effectiveItemKey} => file: {_diffName}, base: {_baseFile}");

            var actual = testCaseContext.GetRequiredItemAsJToken(effectiveItemKey);
            if (!string.IsNullOrEmpty(_jsonPath))
            {
                actual = new JArray(actual.SelectTokens(_jsonPath));
            }

            var testCase = testCaseContext.TestCase;
            var expectedBase = await testCase.GetObjectFromJsonAsync<JToken>(_baseFile);

            var diffs = JsonPatchCreator.Compare(expectedBase, actual, _tokenSelectorProvider, true);
            (var mismatches, var expected) = await testCase.CompareAndRebaseAsync(actual, expectedBase, diffs, _diffName, _tokenSelectorProvider, testCaseContext.Options.IsRebaseline(), true);
            if (mismatches.Count > 0)
            {
                var matchedDiffCategories = testCaseContext.GetDiffCategories(actual, expected, _categoriesFile);
                throw new AssertException("Unexpected token changes:" + string.Join("\n", mismatches.Select(d => d.JsonPath)), matchedDiffCategories);
            }
        }
    }
}
