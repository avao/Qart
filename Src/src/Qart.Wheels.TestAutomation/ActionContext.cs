using Qart.Testing.ActionPipeline;
using System.Net.Http;

namespace Qart.Wheels.TestAutomation
{
    public class ActionContext : PipelineContext, IHttpPipelineContext
    {
        public HttpClient GetHttpClient() => new HttpClient();
    }
}
