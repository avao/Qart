using Qart.Core.DataStore;
using Qart.Testing.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonRemoveAction : IPipelineAction<IPipelineContext>
    {
        private readonly Func<TestCase, IEnumerable<string>> _jsonPathsFunc;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonRemoveAction(string path, object _ = null, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
            : this((testCase) => testCase.GetRequiredAllLines(path), sourceKey, targetKey)
        { }

        public JsonRemoveAction(string jsonPath, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
            : this((testCase) => new[] { jsonPath }, sourceKey, targetKey)
        { }

        private JsonRemoveAction(Func<TestCase, IEnumerable<string>> jsonPathsFunc, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
        {
            _jsonPathsFunc = jsonPathsFunc;
            _sourceKey = sourceKey;
            _targetKey = targetKey;
        }

        public void Execute(TestCaseContext testCaseContext, IPipelineContext context)
        {
            testCaseContext.DescriptionWriter.AddNote("JsonPathExclude", $"{_sourceKey} => {_targetKey}");
            var jtoken = context.GetRequiredItemAsJToken(_sourceKey);
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

            context.SetItem(_targetKey, jtoken);
        }
    }
}
