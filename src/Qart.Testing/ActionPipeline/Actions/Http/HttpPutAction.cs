﻿using Newtonsoft.Json;
using Qart.Core.DataStore;
using Qart.Testing.Context;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpPutAction : IPipelineAction
    {
        private readonly Func<TestCase, TestCaseContext, string> _bodyFunc;
        private readonly string _url;
        private readonly string _targetKey;
        private readonly string _mediaType;
        private readonly string _httpClientKey;

        public HttpPutAction(string url, string sourceKey = null, string targetKey = null, string httpClientKey = ItemKeys.HttpClient, string mediaType = null)
            : this(url, targetKey ?? sourceKey, (testCase, pipelineContext) => pipelineContext.GetRequiredItem(sourceKey), httpClientKey, mediaType)
        { }

        public HttpPutAction(string url, string path, string sourceKey = null, string targetKey = null, string httpClientKey = ItemKeys.HttpClient, string mediaType = null)
            : this(url, targetKey ?? sourceKey, (testCase, pipelineContext) => testCase.GetContent(path), httpClientKey, mediaType)
        { }

        public HttpPutAction(string url, object body, string targetKey = null, string httpClientKey = ItemKeys.HttpClient, string mediaType = null)
            : this(url, targetKey, (testCase, pipelineContext) => JsonConvert.SerializeObject(body), httpClientKey, mediaType)
        { }

        private HttpPutAction(string url, string targetKey, Func<TestCase, TestCaseContext, string> bodyFunc, string httpClientKey, string mediaType)
        {
            _url = url;
            _targetKey = targetKey;
            _bodyFunc = bodyFunc;
            _httpClientKey = httpClientKey;
            _mediaType = mediaType;
        }

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var url = testCaseContext.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpPut", url);
            var body = _bodyFunc(testCaseContext.TestCase, testCaseContext);
            var httpClient = testCaseContext.GetRequiredItem<HttpClient>(_httpClientKey);
            var response = await httpClient.PutEnsureSuccessAsync(url, body, _mediaType, testCaseContext.CorrelationId, testCaseContext.Logger);
            testCaseContext.SetItem(_targetKey, response);
        }
    }
}
