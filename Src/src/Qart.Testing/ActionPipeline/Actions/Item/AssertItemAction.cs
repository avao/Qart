using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertItemAction : IPipelineAction<IPipelineContext>
    {
        private readonly string _path;
        private readonly string _key;

        public AssertItemAction(string path, string key = PipelineContextKeys.Content)
        {
            _path = path;
            _key = key;
        }

        public void Execute(TestCaseContext testCaseContext, IPipelineContext context)
        {
            testCaseContext.DescriptionWriter.AddNote("AssertItem", $"{_key}");
            testCaseContext.AssertContent(context.GetRequiredItemAsString(_key), _path);
        }
    }
}
