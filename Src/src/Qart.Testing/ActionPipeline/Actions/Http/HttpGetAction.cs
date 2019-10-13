using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpGetAction<T> : IPipelineAction<T>
        where T : IHttpPipelineContext
    {
        private readonly string _url;
        private readonly string _itemKey;

        public HttpGetAction(string url, string itemKey = PipelineContextKeys.Content)
        {
            _url = url;
            _itemKey = itemKey;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            var url = context.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpGet", url);
            context.SetItem(_itemKey, context.GetRequiredHttpClient().GetEnsureSuccess(url, testCaseContext.Logger));
        }
    }
}
