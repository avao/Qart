using Qart.Core.Validation;
using System.Net.Http;

namespace Qart.Testing.ActionPipeline
{
    public interface IHttpPipelineContext : IPipelineContext
    {
        HttpClient GetHttpClient();
    }

    public static class HttpPipelineContextExtensions
    {
        public static HttpClient GetRequiredHttpClient(this IHttpPipelineContext context)
        {
            var httpClient = context.GetHttpClient();
            Require.NotNull(httpClient, "HttpClient mustnot be null");
            return httpClient;
        }
    }
}
