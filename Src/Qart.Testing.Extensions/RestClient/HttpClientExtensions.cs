using Common.Logging;
using NUnit.Framework;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Extensions.RestClient
{
    public static class HttpClientExtensions
    {
        public static string GetEnsureSuccess(this HttpClient client, string relativeUri, ILog logger)
        {
            var result = client.GetAsync(relativeUri).Result;
            result.AssertSuccessStatusCode(logger);
            return result.Content.ReadAsStringAsync().Result;
        }

        public static string PostEnsureSuccess(this HttpClient client, string relativeUri, string content, ILog logger)
        {
            var result = client.PostJsonAsync(relativeUri, content).Result;
            result.AssertSuccessStatusCode(logger);
            return result.Content.ReadAsStringAsync().Result;
        }

        public static Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string relativeUri, string content)
        {
            return client.PostAsync(relativeUri, new StringContent(content, Encoding.UTF8, "application/json"));
        }
        
        private static void AssertSuccessStatusCode(this HttpResponseMessage message, ILog logger)
        {
            if (!message.IsSuccessStatusCode)
            {
                logger.Error(message.Content.ReadAsStringAsync().Result);
                Assert.That(message.IsSuccessStatusCode, Is.EqualTo(true));
            }
        }
    }
}
