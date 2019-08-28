using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class HttpGetAction<T> : IPipelineAction<T>
        where T : IHttpContext
    {
        private string _url;

        public HttpGetAction(string url)
        {
            _url = url;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            context.Content = context.HttpClient.GetEnsureSuccess(_url, testCaseContext.Logger);
        }
    }
}
