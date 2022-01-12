using Qart.Testing.Framework;
using System.ComponentModel;
using System.Net.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    [Description("Http")]
    public class HttpNoBodyAction : HttpAction
    {
        public HttpNoBodyAction(HttpMethod httpMethod, [Description("relative url")] string url, string targetKey = null, IHttpRequestMessageProcessor httpRequestMessageProcessor = null)
            : base(httpMethod, url, targetKey, (_, __) => new HttpContent[] { null }, httpRequestMessageProcessor, false)
        {
        }
    }
}
