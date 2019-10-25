using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System.Net.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpDeleteAction : IPipelineAction
    {
        private string _url;
        private string _httpClientKey;

        public HttpDeleteAction(string url, string httpClientKey = ItemKeys.HttpClient)
        {
            _url = url;
            _httpClientKey = httpClientKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var url = testCaseContext.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpDelete", url);
            var httpClient = testCaseContext.GetRequiredItem<HttpClient>(_httpClientKey);
            httpClient.DeleteEnsureSuccess(url, testCaseContext.Logger);
        }
    }
}
