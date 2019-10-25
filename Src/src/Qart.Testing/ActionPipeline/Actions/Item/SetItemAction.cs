using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class SetItemAction : IPipelineAction
    {
        private readonly string _key;
        private readonly string _value;

        public SetItemAction(string value, string key = ItemKeys.Content)
        {
            _key = key;
            _value = value;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("SetItem", $"{_key}");
            testCaseContext.SetItem(_key, _value);
        }
    }
}
