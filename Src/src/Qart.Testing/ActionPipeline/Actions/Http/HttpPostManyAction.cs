using Qart.Core.DataStore;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpPostManyAction<T> : IPipelineAction<T>
        where T : IHttpPipelineContext
    {
        private readonly Func<TestCase, IPipelineContext, IEnumerable<string>> _bodyFunc;
        private readonly string _url;
        private readonly string _targetKey;

        public HttpPostManyAction(string url, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
            : this(url, targetKey, (testCase, pipelineContext) => pipelineContext.GetRequiredItem<IEnumerable<string>>(sourceKey))
        { }

        public HttpPostManyAction(string path, string url, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
            : this(url, targetKey, (testCase, pipelineContext) => testCase.GetItemIds(path).Select(id => testCase.GetContent(id)))
        { }

        private HttpPostManyAction(string url, string targetKey, Func<TestCase, IPipelineContext, IEnumerable<string>> bodyFunc)
        {
            _url = url;
            _targetKey = targetKey;
            _bodyFunc = bodyFunc;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            testCaseContext.DescriptionWriter.AddNote("HttpPostMany", _url);
            var httpClient = context.GetRequiredHttpClient();
            var responses = _bodyFunc(testCaseContext.TestCase, context).Select(body => httpClient.PostEnsureSuccess(_url, body, testCaseContext.Logger)).ToList();
            context.SetItem(_targetKey, responses);
        }
    }
}
