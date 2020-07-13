using Qart.Core.DataStore;
using Qart.Testing.Diff;
using Qart.Testing.Framework;
using System;
using System.Collections.Generic;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonRemoveAction : IPipelineAction
    {
        private readonly Func<TestCase, IEnumerable<string>> _jsonPathsFunc;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonRemoveAction(string path, object _ = null, string sourceKey = null, string targetKey = null)
            : this((testCase) => testCase.GetRequiredAllLines(path), sourceKey, targetKey)
        { }

        public JsonRemoveAction(string jsonPath, string sourceKey = null, string targetKey = null)
            : this((testCase) => new[] { jsonPath }, sourceKey, targetKey)
        { }

        private JsonRemoveAction(Func<TestCase, IEnumerable<string>> jsonPathsFunc, string sourceKey = null, string targetKey = null)
        {
            _jsonPathsFunc = jsonPathsFunc;
            _sourceKey = sourceKey;
            _targetKey = targetKey ?? sourceKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveSourceKey = testCaseContext.GetEffectiveItemKey(_sourceKey);
            var effectiveTargetKey = testCaseContext.GetEffectiveItemKey(_targetKey);

            testCaseContext.DescriptionWriter.AddNote("JsonPathExclude", $"{effectiveSourceKey} => {effectiveTargetKey}");
            var itemToken = testCaseContext.GetRequiredItemAsJToken(effectiveSourceKey);
            if (effectiveSourceKey != effectiveTargetKey)
            {
                itemToken = itemToken.DeepClone();
            }

            itemToken.RemoveTokens(_jsonPathsFunc(testCaseContext.TestCase));

            testCaseContext.SetItem(effectiveTargetKey, itemToken);
        }
    }
}
