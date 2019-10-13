using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpDeleteAction<T> : IPipelineAction<T>
        where T : IHttpPipelineContext
    {
        private string _url;

        public HttpDeleteAction(string url)
        {
            _url = url;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            var url = context.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpDelete", url);
            context.GetRequiredHttpClient().DeleteEnsureSuccess(url, testCaseContext.Logger);
        }
    }
}
