using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Framework.Http
{
    public static class HttpClientExtensions
    {
        public static Task<string> GetEnsureSuccessAsync(this HttpClient client, string relativeUri, string correlationId, ILogger logger)
            => client.SendEnsureSuccessAsync(HttpMethod.Get, relativeUri, null, correlationId, logger);

        public static Task<string> PostEnsureSuccessAsync(this HttpClient client, string relativeUri, string content, string mediaType, string correlationId, ILogger logger)
            => client.SendEnsureSuccessAsync(HttpMethod.Post, relativeUri, CreateContent(content, mediaType), correlationId, logger);

        public static Task<string> PutEnsureSuccessAsync(this HttpClient client, string relativeUri, string content, string mediaType, string correlationId, ILogger logger)
            => client.SendEnsureSuccessAsync(HttpMethod.Put, relativeUri, CreateContent(content, mediaType), correlationId, logger);

        public static Task<string> DeleteEnsureSuccessAsync(this HttpClient client, string relativeUri, string correlationId, ILogger logger)
            => client.SendEnsureSuccessAsync(HttpMethod.Delete, relativeUri, null, correlationId, logger);


        public static StringContent CreateContent(string content, string mediaType)
        {
            if (content == null)
                return null;

            mediaType ??= content.StartsWith("<?xml")
                ? "application/xml"
                : "application/json";
            return new StringContent(content, Encoding.UTF8, mediaType);
        }

        private static async Task<string> SendEnsureSuccessAsync(this HttpClient client, HttpMethod httpMethod, string relativeUri, HttpContent content, string correlationId, ILogger logger)
        {
            var request = CreateHttpRequestMessage(httpMethod, relativeUri, content, correlationId);
            return await client.SendEnsureSuccessAsync(request, logger);
        }

        public static HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, string relativeUri, HttpContent content, string correlationId)
        {
            var request = new HttpRequestMessage(httpMethod, relativeUri);
            request.Headers.Add("X-Correlation-ID", correlationId);
            if (content != null)
            {
                request.Content = content;
            }
            return request;
        }

        public static async Task<string> SendEnsureSuccessAsync(this HttpClient client, HttpRequestMessage httpRequestMessage, ILogger logger)
        {
            logger.LogDebug("Executing {HttpMethod} for {Url}", httpRequestMessage.Method, httpRequestMessage.RequestUri);
            if (httpRequestMessage.Content != null)
            {
                logger.LogTrace("with content: {content}", httpRequestMessage.Content.ReadAsStringAsync().Result);
            }
            return await client.SendAsync(httpRequestMessage).GetContentAssertSuccessAsync(logger);
        }
    }
}
