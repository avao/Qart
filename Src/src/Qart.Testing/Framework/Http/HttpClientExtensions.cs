using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Framework.Http
{
    public static class HttpClientExtensions
    {
        public static Task<string> GetEnsureSuccessAsync(this HttpClient client, string relativeUri, ILogger logger)
        {
            logger.LogDebug("Executing GET on [{0}]", relativeUri);
            return client.GetAsync(relativeUri).GetContentAssertSuccessAsync(logger);
        }

        public static string GetEnsureSuccess(this HttpClient client, string relativeUri, ILogger logger)
        {
            return client.GetEnsureSuccessAsync(relativeUri, logger).GetAwaiter().GetResult();
        }

        public static Task<string> PostEnsureSuccessAsync(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            if (content.StartsWith("<?xml"))
                return client.PostXmlEnsureSuccessAsync(relativeUri, content, logger);
            else
                return client.PostJsonEnsureSuccessAsync(relativeUri, content, logger);
        }

        public static string PostEnsureSuccess(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            return client.PostEnsureSuccessAsync(relativeUri, content, logger).GetAwaiter().GetResult();
        }

        public static Task<string> DeleteEnsureSuccessAsync(this HttpClient client, string relativeUri, ILogger logger)
        {
            logger.LogDebug("Executing DELETE on [{0}]", relativeUri);
            return client.DeleteAsync(relativeUri).GetContentAssertSuccessAsync(logger);
        }

        public static string DeleteEnsureSuccess(this HttpClient client, string relativeUri, ILogger logger)
        {
            return client.DeleteEnsureSuccessAsync(relativeUri, logger).GetAwaiter().GetResult();
        }

        public static Task<string> PostJsonEnsureSuccessAsync(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            return client.PostJsonAsync(relativeUri, content, logger).GetContentAssertSuccessAsync(logger);
        }

        public static Task<string> PostXmlEnsureSuccessAsync(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            return client.PostXmlAsync(relativeUri, content, logger).GetContentAssertSuccessAsync(logger);
        }

        public static Task<HttpResponseMessage> PostXmlAsync(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            return client.PostAsync(relativeUri, content, "application/xml", logger);
        }

        public static Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            return client.PostAsync(relativeUri, content, "application/json", logger);
        }

        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string relativeUri, string content, string mediaType, ILogger logger)
        {
            logger.LogDebug("Executing POST on [{0}]", relativeUri);
            return client.PostAsync(relativeUri, new StringContent(content, Encoding.UTF8, mediaType));
        }
    }
}
