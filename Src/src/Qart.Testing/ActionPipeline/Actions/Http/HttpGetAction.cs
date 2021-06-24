using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System.Net.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpGetAction : IPipelineAction
    {
        private readonly string _url;
        private readonly string _itemKey;
        private readonly string _httpClientKey;

        public HttpGetAction(string url, string itemKey = null, string httpClientKey = ItemKeys.HttpClient)
        {
            _url = url;
            _itemKey = itemKey;
            _httpClientKey = httpClientKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var url = testCaseContext.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpGet", url);
            var httpClient = testCaseContext.GetRequiredItem<HttpClient>(_httpClientKey);
            testCaseContext.SetItem(_itemKey, httpClient.GetEnsureSuccess(url, testCaseContext.Logger));
        }
    }
}
