using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class AssertJsonContentAction<T> : IPipelineAction<T>
        where T : IHttpContext
    {
        private readonly string _fileName;

        public AssertJsonContentAction(string fileName)
        {
            _fileName = fileName;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            testCaseContext.AssertContentJson(context.Content, _fileName);
        }
    }
}
