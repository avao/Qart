using Newtonsoft.Json.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class ToJTokenAction : IPipelineAction
    {
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public ToJTokenAction(string sourceKey = ItemKeys.Content, string targetKey = ItemKeys.Content)
        {
            _sourceKey = sourceKey;
            _targetKey = targetKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("ToJToken", $"{_sourceKey} => {_targetKey}");
            string content = testCaseContext.GetRequiredItem<string>(_sourceKey);
            testCaseContext.SetItem(_targetKey, JToken.Parse(content));
        }
    }
}
