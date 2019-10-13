using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class ToJTokenAction : IPipelineAction<IPipelineContext>
    {
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public ToJTokenAction(string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
        {
            _sourceKey = sourceKey;
            _targetKey = targetKey;
        }

        public void Execute(TestCaseContext testCaseContext, IPipelineContext context)
        {
            testCaseContext.DescriptionWriter.AddNote("ToJToken", $"{_sourceKey} => {_targetKey}");
            string content = context.GetRequiredItem<string>(_sourceKey);
            context.SetItem(_targetKey, JToken.Parse(content));
        }
    }
}
