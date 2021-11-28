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


        private static StringContent CreateContent(string content, string mediaType)
        {
            mediaType ??= content.StartsWith("<?xml")
                ? "application/xml"
                : "application/json";
            return new StringContent(content, Encoding.UTF8, mediaType);
        }

        private static async Task<string> SendEnsureSuccessAsync(this HttpClient client, HttpMethod httpMethod, string relativeUri, StringContent content, string correlationId, ILogger logger)
        {
            logger.LogDebug("Executing {HttpMethod} on {Url}", httpMethod, relativeUri);
            var request = new HttpRequestMessage(httpMethod, relativeUri);
            request.Headers.Add("X-Correlation-ID", correlationId);
            if (content != null)
            {
                logger.LogTrace("with content: {content}", content.ReadAsStringAsync().Result);
                request.Content = content;
            }
            return await client.SendAsync(request).GetContentAssertSuccessAsync(logger);
        }
    }
}
