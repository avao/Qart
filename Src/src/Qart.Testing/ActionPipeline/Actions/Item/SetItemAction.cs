using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class SetItemAction : IPipelineAction<IPipelineContext>
    {
        private readonly string _key;
        private readonly string _value;

        public SetItemAction(string value, string key = PipelineContextKeys.Content)
        {
            _key = key;
            _value = value;
        }

        public void Execute(TestCaseContext testCaseContext, IPipelineContext context)
        {
            testCaseContext.DescriptionWriter.AddNote("SetItem", $"{_key}");
            context.SetItem(_key, _value);
        }
    }
}
