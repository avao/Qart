using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class SetItemAction : IPipelineAction
    {
        private readonly string _key;
        private readonly string _value;

        public SetItemAction(string value, string key = null)
        {
            _key = key;
            _value = value;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);
            var value = testCaseContext.Resolve(_value);

            testCaseContext.DescriptionWriter.AddNote("SetItem", $"{effectiveItemKey} = {value}");
            testCaseContext.SetItem(effectiveItemKey, value);
        }
    }
}
