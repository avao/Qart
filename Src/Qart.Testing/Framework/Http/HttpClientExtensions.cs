using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Framework.Http
{
    public static class HttpClientExtensions
    {
        public static string GetEnsureSuccess(this HttpClient client, string relativeUri)
        {
            return client.GetAsync(relativeUri).GetContentEnsureSuccess();
        }

        public static string PostEnsureSuccess(this HttpClient client, string relativeUri, string content)
        {
            if (content.StartsWith("<?xml"))
                return client.PostXmlEnsureSuccess(relativeUri, content);
            else
                return client.PostJsonEnsureSuccess(relativeUri, content);
        }

        public static string PostJsonEnsureSuccess(this HttpClient client, string relativeUri, string content)
        {
            return client.PostJsonAsync(relativeUri, content).GetContentEnsureSuccess();
        }

        public static string PostXmlEnsureSuccess(this HttpClient client, string relativeUri, string content)
        {
            return client.PostXmlAsync(relativeUri, content).GetContentEnsureSuccess();
        }

        public static Task<HttpResponseMessage> PostXmlAsync(this HttpClient client, string relativeUri, string content)
        {
            return client.PostAsync(relativeUri, content, "application/xml");
        }

        public static Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string relativeUri, string content)
        {
            return client.PostAsync(relativeUri, content, "application/json");
        }

        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string relativeUri, string content, string mediaType)
        {
            return client.PostAsync(relativeUri, new StringContent(content, Encoding.UTF8, mediaType));
        }
    }
}
