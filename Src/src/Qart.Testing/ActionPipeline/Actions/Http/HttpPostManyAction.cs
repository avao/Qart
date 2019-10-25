using Qart.Core.DataStore;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpPostManyAction : IPipelineAction
    {
        private readonly Func<TestCase, TestCaseContext, IEnumerable<string>> _bodyFunc;
        private readonly string _url;
        private readonly string _targetKey;
        private string _httpClientKey;

        public HttpPostManyAction(string url, string sourceKey = ItemKeys.Content, string targetKey = ItemKeys.Content, string httpClientKey = ItemKeys.HttpClient)
            : this(url, targetKey, (testCase, pipelineContext) => pipelineContext.GetRequiredItem<IEnumerable<string>>(sourceKey), httpClientKey)
        { }

        public HttpPostManyAction(string path, string url, string sourceKey = ItemKeys.Content, string targetKey = ItemKeys.Content, string httpClientKey = ItemKeys.HttpClient)
            : this(url, targetKey, (testCase, pipelineContext) => testCase.GetItemIds(path).Select(id => testCase.GetContent(id)), httpClientKey)
        { }

        private HttpPostManyAction(string url, string targetKey, Func<TestCase, TestCaseContext, IEnumerable<string>> bodyFunc, string httpClientKey)
        {
            _url = url;
            _targetKey = targetKey;
            _bodyFunc = bodyFunc;
            _httpClientKey = httpClientKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            testCaseContext.DescriptionWriter.AddNote("HttpPostMany", _url);
            var httpClient = testCaseContext.GetRequiredItem<HttpClient>(_httpClientKey);
            var responses = _bodyFunc(testCaseContext.TestCase, testCaseContext).Select(body => httpClient.PostEnsureSuccess(_url, body, testCaseContext.Logger)).ToList();
            testCaseContext.SetItem(_targetKey, responses);
        }
    }
}
