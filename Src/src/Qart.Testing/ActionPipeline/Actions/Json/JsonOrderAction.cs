using Qart.Testing.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonOrderAction : IPipelineAction<IPipelineContext>
    {
        private readonly IEnumerable<string> _jsonPaths;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonOrderAction(IEnumerable<string> jsonPaths, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
        {
            _jsonPaths = jsonPaths;
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
            foreach (var token in _jsonPaths.SelectMany(jsonPath => jtoken.SelectTokens(jsonPath)))
            {
                token.Remove();
            }
            context.SetItem(_targetKey, jtoken);
        }
    }
}
