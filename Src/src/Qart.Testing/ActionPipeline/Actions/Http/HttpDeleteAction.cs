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
            testCaseContext.DescriptionWriter.AddNote("HttpDelete", _url);
            context.GetRequiredHttpClient().DeleteEnsureSuccess(_url, testCaseContext.Logger);
        }
    }
}
