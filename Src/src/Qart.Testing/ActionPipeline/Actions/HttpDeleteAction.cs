using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class HttpDeleteAction<T> : IPipelineAction<T>
        where T : IHttpContext
    {
        private string _url;

        public HttpDeleteAction(string url)
        {
            _url = url;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            context.HttpClient.DeleteEnsureSuccess(_url, testCaseContext.Logger);
        }
    }
}
