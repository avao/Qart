using Qart.Core.DataStore;
using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class LoadItemAction : IPipelineAction<IPipelineContext>
    {
        private readonly string _key;
        private readonly string _path;

        public LoadItemAction(string path, string key = PipelineContextKeys.Content)
        {
            _key = key;
            _path = path;
        }

        public void Execute(TestCaseContext testCaseContext, IPipelineContext context)
        {
            testCaseContext.DescriptionWriter.AddNote("LoadItem", $"file:{_path} => {_key}");
            var content = testCaseContext.TestCase.GetContent(_path);
            context.SetItem(_key, content);
        }
    }
}
