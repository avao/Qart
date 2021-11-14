using Qart.Testing.Context;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpDeleteAction : IPipelineAction
    {
        private readonly string _url;
        private readonly string _httpClientKey;

        public HttpDeleteAction(string url, string httpClientKey = ItemKeys.HttpClient)
        {
            _url = url;
            _httpClientKey = httpClientKey;
        }

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var url = testCaseContext.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpDelete", url);
            var httpClient = testCaseContext.GetRequiredItem<HttpClient>(_httpClientKey);
            await httpClient.DeleteEnsureSuccessAsync(url, testCaseContext.Logger);
        }
    }
}
