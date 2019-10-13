using Qart.Core.DataStore;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class SaveItemAction : IPipelineAction<IPipelineContext>
    {
        private readonly string _key;
        private readonly string _path;

        public SaveItemAction(string path, string key = PipelineContextKeys.Content)
        {
            _key = key;
            _path = path;
        }

        public void Execute(TestCaseContext testCaseContext, IPipelineContext context)
        {
            testCaseContext.DescriptionWriter.AddNote("SaveItem", $"{_key} => file: {_path}");
            testCaseContext.TestCase.PutContent(_path, context.GetRequiredItemAsString(_key));
        }
    }
}
