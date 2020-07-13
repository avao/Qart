using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Item
{
    public class SetItemAction : IPipelineAction
    {
        private readonly string _key;
        private readonly string _value;
        private readonly bool _resolve;

        public SetItemAction(string value, string key = null, bool resolve = true)
        {
            _key = key;
            _value = value;
            _resolve = resolve;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveItemKey = testCaseContext.GetEffectiveItemKey(_key);

            var value = _resolve
                ? testCaseContext.Resolve(_value)
                : _value;

            testCaseContext.DescriptionWriter.AddNote("SetItem", $"{effectiveItemKey} = {value}");

            testCaseContext.SetItem(effectiveItemKey, value);
        }
    }
}
