using Qart.Testing.Context;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    [Description("Http")]
    public class HttpGetAction : IPipelineAction
    {
        private readonly string _url;
        private readonly string _itemKey;
        private readonly string _httpClientKey;

        public HttpGetAction([Description("relative url")]string url, string itemKey = null, string httpClientKey = ItemKeys.HttpClient)
        {
            _url = url;
            _itemKey = itemKey;
            _httpClientKey = httpClientKey;
        }

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var url = testCaseContext.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpGet", url);
            var httpClient = testCaseContext.GetRequiredItem<HttpClient>(_httpClientKey);
            var response = await httpClient.GetEnsureSuccessAsync(url, testCaseContext.Logger);
            testCaseContext.SetItem(_itemKey, response);
        }
    }
}
