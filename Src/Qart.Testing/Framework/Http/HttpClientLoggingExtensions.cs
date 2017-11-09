using Common.Logging;
using System.Net.Http;

namespace Qart.Testing.Framework.Http
{
    public static class HttpClientLoggingExtensions
    {
        public static string GetEnsureSuccess(this HttpClient client, string relativeUri, ILog logger)
        {
            logger.InfoFormat("Executing GET on [{0}]", relativeUri);
            return client.GetEnsureSuccess(relativeUri);
        }

        public static string PostEnsureSuccess(this HttpClient client, string relativeUri, string content, ILog logger)
        {
            logger.InfoFormat("Executing POST on [{0}]", relativeUri);
            return client.PostEnsureSuccess(relativeUri, content);
        }

        public static string DeleteEnsureSuccess(this HttpClient client, string relativeUri, ILog logger)
        {
            logger.InfoFormat("Executing DELETE on [{0}]", relativeUri);
            return client.DeleteAsync(relativeUri).GetContentEnsureSuccess();
        }
    }
}
