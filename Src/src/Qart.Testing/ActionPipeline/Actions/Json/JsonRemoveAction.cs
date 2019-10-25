using Qart.Core.DataStore;
using Qart.Testing.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonRemoveAction : IPipelineAction
    {
        private readonly Func<TestCase, IEnumerable<string>> _jsonPathsFunc;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonRemoveAction(string path, object _ = null, string sourceKey = ItemKeys.Content, string targetKey = ItemKeys.Content)
            : this((testCase) => testCase.GetRequiredAllLines(path), sourceKey, targetKey)
        { }

        public JsonRemoveAction(string jsonPath, string sourceKey = ItemKeys.Content, string targetKey = ItemKeys.Content)
            : this((testCase) => new[] { jsonPath }, sourceKey, targetKey)
        { }

        private JsonRemoveAction(Func<TestCase, IEnumerable<string>> jsonPathsFunc, string sourceKey = ItemKeys.Content, string targetKey = ItemKeys.Content)
        {
            _jsonPathsFunc = jsonPathsFunc;
            _sourceKey = sourceKey;
            _targetKey = targetKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("JsonPathExclude", $"{_sourceKey} => {_targetKey}");
            var jtoken = testCaseContext.GetRequiredItemAsJToken(_sourceKey);
            if (_sourceKey != _targetKey)
            {
                jtoken = jtoken.DeepClone();
            }

            var tokensToRemove = _jsonPathsFunc(testCaseContext.TestCase)
                .SelectMany(jsonPath => jtoken.SelectTokens(jsonPath))
                .ToList();

            foreach (var token in tokensToRemove)
            {
                token.Remove();
            }

            testCaseContext.SetItem(_targetKey, jtoken);
        }
    }
}
