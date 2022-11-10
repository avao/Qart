using Newtonsoft.Json;
using Qart.Core.DataStore;
using Qart.Testing.Context;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpAction : IPipelineAction
    {
        private static readonly HttpClient _httpClient = new();

        private readonly Func<TestCase, TestCaseContext, HttpContent[]> _bodyFunc;
        private readonly HttpMethod _httpMethod;
        private readonly string _url;
        private readonly string _targetKey;
        private readonly IHttpRequestMessageProcessor _httpRequestMessageProcessor;
        private readonly bool _multipleResults;

        public HttpAction(HttpMethod httpMethod, string url, string sourceKey = null, string targetKey = null, string mediaType = null, IHttpRequestMessageProcessor httpRequestMessageProcessor = null)
            : this(httpMethod, url, targetKey ?? sourceKey, (testCase, pipelineContext) => new[] { HttpClientExtensions.CreateContent(pipelineContext.TryGetItem(sourceKey, out object item) ? item.ToString() : null, mediaType) }, httpRequestMessageProcessor, false)
        { }

        public HttpAction(HttpMethod httpMethod, string url, string path, string sourceKey = null, string targetKey = null, string mediaType = null, IHttpRequestMessageProcessor httpRequestMessageProcessor = null)
            : this(httpMethod, url, targetKey ?? sourceKey, (testCase, pipelineContext) => new[] { HttpClientExtensions.CreateContent(testCase.GetContent(path), mediaType) }, httpRequestMessageProcessor, false)
        { }

        public HttpAction(HttpMethod httpMethod, string url, object body, string targetKey = null, string mediaType = null, IHttpRequestMessageProcessor httpRequestMessageProcessor = null)
            : this(httpMethod, url, targetKey, (testCase, pipelineContext) => new[] { HttpClientExtensions.CreateContent(JsonConvert.SerializeObject(body), mediaType) }, httpRequestMessageProcessor, false)
        { }

        protected HttpAction(HttpMethod httpMethod, string url, string targetKey, Func<TestCase, TestCaseContext, HttpContent[]> bodyFunc, IHttpRequestMessageProcessor httpRequestMessageProcessor, bool multipleResults)
        {
            _url = url;
            _targetKey = targetKey;
            _bodyFunc = bodyFunc;
            _httpMethod = httpMethod;
            _httpRequestMessageProcessor = httpRequestMessageProcessor;
            _multipleResults = multipleResults;
        }

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var url = testCaseContext.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote($"Http{_httpMethod}", url);

            List<string> results = null;
            foreach (var body in _bodyFunc(testCaseContext.TestCase, testCaseContext)) //TODO check suppress exceptions
            {
                var request = HttpClientExtensions.CreateHttpRequestMessage(_httpMethod, url, body, testCaseContext.CorrelationId);
                _httpRequestMessageProcessor?.Process(testCaseContext, request);
                var response = await _httpClient.SendEnsureSuccessAsync(request, testCaseContext.Logger);

                if (!_multipleResults)
                {
                    testCaseContext.SetItem(_targetKey, response);
                }
                else
                {
                    results ??= new List<string>();
                    results.Add(response);
                }
            }

            if (results != null)
            {
                testCaseContext.SetItem(_targetKey, results);
            }
        }
    }
}
