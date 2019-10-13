using Newtonsoft.Json;
using Qart.Core.DataStore;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpPostAction<T> : IPipelineAction<T>
        where T : IHttpPipelineContext
    {
        private readonly Func<TestCase, IPipelineContext, string> _bodyFunc;
        private readonly string _url;
        private readonly string _targetKey;

        public HttpPostAction(string url, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
            : this(url, targetKey, (testCase, pipelineContext) => pipelineContext.GetRequiredItemAsString(sourceKey))
        { }

        public HttpPostAction(string url, string path, string sourceKey = PipelineContextKeys.Content, string targetKey = PipelineContextKeys.Content)
            : this(url, targetKey, (testCase, pipelineContext) => testCase.GetContent(path))
        { }

        public HttpPostAction(string url, object body, string targetKey = PipelineContextKeys.Content)
            : this(url, targetKey, (testCase, pipelineContext) => JsonConvert.SerializeObject(body))
        { }

        private HttpPostAction(string url, string targetKey, Func<TestCase, IPipelineContext, string> bodyFunc)
        {
            _url = url;
            _targetKey = targetKey;
            _bodyFunc = bodyFunc;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            var url = context.Resolve(_url);
            testCaseContext.DescriptionWriter.AddNote("HttpPost", url);
            var body = _bodyFunc(testCaseContext.TestCase, context);
            context.SetItem(_targetKey, context.GetRequiredHttpClient().PostEnsureSuccess(url, body, testCaseContext.Logger));
        }
    }
}
