using Newtonsoft.Json;
using Qart.Core.DataStore;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System;
using System.Net.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpPostAction : IPipelineAction
    {
        private readonly Func<TestCase, TestCaseContext, string> _bodyFunc;
        private readonly string _url;
        private readonly string _targetKey;
        private string _httpClientKey;

        public HttpPostAction(string url, string sourceKey = null, string targetKey = null, string httpClientKey = ItemKeys.HttpClient)
            : this(url, targetKey ?? sourceKey, (testCase, pipelineContext) => pipelineContext.GetRequiredItem(sourceKey), httpClientKey)
        { }

        public HttpPostAction(string url, string path, string sourceKey = null, string targetKey = null, string httpClientKey = ItemKeys.HttpClient)
            : this(url, targetKey ?? sourceKey, (testCase, pipelineContext) => testCase.GetContent(path), httpClientKey)
        { }

        public HttpPostAction(string url, object body, string targetKey = null, string httpClientKey = ItemKeys.HttpClient)
            : this(url, targetKey, (testCase, pipelineContext) => JsonConvert.SerializeObject(body), httpClientKey)
        { }

        private HttpPostAction(string url, string targetKey, Func<TestCase, TestCaseContext, string> bodyFunc, string httpClientKey)
        {
            _url = url;
            _targetKey = targetKey;
            _bodyFunc = bodyFunc;
            _httpClientKey = httpClientKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var url = testCaseContext.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpPost", url);
            var body = _bodyFunc(testCaseContext.TestCase, testCaseContext);
            var httpClient = testCaseContext.GetRequiredItem<HttpClient>(_httpClientKey);
            testCaseContext.SetItem(_targetKey, httpClient.PostEnsureSuccess(url, body, testCaseContext.Logger));
        }
    }
}
