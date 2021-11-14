using Qart.Core.DataStore;
using Qart.Testing.Context;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpPostManyAction : SyncActionBase
    {
        private readonly Func<TestCase, TestCaseContext, IEnumerable<string>> _bodyFunc;
        private readonly string _url;
        private readonly string _targetKey;
        private readonly string _mediaType;
        private readonly string _httpClientKey;

        public HttpPostManyAction(string url, string sourceKey = null, string targetKey = null, string httpClientKey = ItemKeys.HttpClient, string mediaType = null)
            : this(url, targetKey ?? sourceKey, (testCase, pipelineContext) => pipelineContext.GetRequiredItem<IEnumerable<string>>(sourceKey), httpClientKey, mediaType)
        { }

        public HttpPostManyAction(string path, string url, string sourceKey = null, string targetKey = null, string httpClientKey = ItemKeys.HttpClient, string mediaType = null)
            : this(url, targetKey ?? sourceKey, (testCase, pipelineContext) => testCase.GetItemIds(path).Select(id => testCase.GetContent(id)), httpClientKey, mediaType)
        { }

        private HttpPostManyAction(string url, string targetKey, Func<TestCase, TestCaseContext, IEnumerable<string>> bodyFunc, string httpClientKey, string mediaType)
        {
            _url = url;
            _targetKey = targetKey;
            _bodyFunc = bodyFunc;
            _httpClientKey = httpClientKey;
            _mediaType = mediaType;
        }

        public override void Execute(TestCaseContext testCaseContext)
        {
            var url = testCaseContext.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpPostMany", _url);
            var httpClient = testCaseContext.GetRequiredItem<HttpClient>(_httpClientKey);
            var responses = _bodyFunc(testCaseContext.TestCase, testCaseContext)
                .Select(async (body) => await httpClient.PostEnsureSuccessAsync(url, body, _mediaType, testCaseContext.Logger))
                .ToList();
            testCaseContext.SetItem(_targetKey, responses);
        }
    }
}
