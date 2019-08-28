using System.Net.Http;

namespace Qart.Testing.ActionPipeline
{
    public interface IHttpContext
    {
        HttpClient HttpClient { get; }

        string Content { get; set; }
    }
}
