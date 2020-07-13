﻿using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;
using System.Collections.Generic;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertContentJsonAction : IPipelineAction
    {
        private readonly string _fileName;
        private readonly string _jsonPath;
        private readonly string _itemKey;
        private readonly string _replaceGroups;

        public AssertContentJsonAction(string fileName, string jsonPath = null, string itemKey = null, string replaceGroups = null)
        {
            _fileName = fileName;
            _jsonPath = jsonPath;
            _itemKey = itemKey;
            _replaceGroups = replaceGroups;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_itemKey);
            testCaseContext.DescriptionWriter.AddNote("AssertContent", $"{effectiveItemKey} => file:{_fileName}");

            var token = testCaseContext.GetRequiredItemAsJToken(_itemKey);
            if (!string.IsNullOrEmpty(_jsonPath))
            {
                var jsonPath = testCaseContext.Resolve(_jsonPath);
                token = new JArray(token.SelectTokens(jsonPath));
            }


            if (_replaceGroups != null)
            {
                token = token.DeepClone();

                var groups = testCaseContext.TestCase.GetObjectFromJson<Dictionary<string, IReadOnlyCollection<string>>>(_replaceGroups);

                testCaseContext.ReplaceTokens(token, groups);
            }

            testCaseContext.AssertContent(token, _fileName);
        }
    }
}
