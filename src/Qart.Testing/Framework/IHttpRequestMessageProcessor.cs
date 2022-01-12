using System.Net.Http;

namespace Qart.Testing.Framework
{
    public interface IHttpRequestMessageProcessor
    {
        void Process(TestCaseContext testCaseContext, HttpRequestMessage request);
    }
}
