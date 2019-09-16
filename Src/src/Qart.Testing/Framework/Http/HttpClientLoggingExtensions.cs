using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Qart.Testing.Framework.Http
{
    public static class HttpClientLoggingExtensions
    {
        public static string GetEnsureSuccess(this HttpClient client, string relativeUri, ILogger logger)
        {
            logger.LogInformation("Executing GET on [{0}]", relativeUri);
            return client.GetEnsureSuccess(relativeUri);
        }

        public static string PostEnsureSuccess(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            logger.LogInformation("Executing POST on [{0}]", relativeUri);
            return client.PostEnsureSuccess(relativeUri, content);
        }

        public static string DeleteEnsureSuccess(this HttpClient client, string relativeUri, ILogger logger)
        {
            logger.LogInformation("Executing DELETE on [{0}]", relativeUri);
            return client.DeleteAsync(relativeUri).GetContentEnsureSuccess();
        }
    }
}
