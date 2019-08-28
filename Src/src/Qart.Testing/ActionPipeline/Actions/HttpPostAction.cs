using Common.Logging;
using Newtonsoft.Json;
using Qart.Core.DataStore;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class HttpPostAction<T> : IPipelineAction<T>
        where T : IHttpContext
    {
        private readonly string _body;
        private readonly string _path;
        private readonly string _url;

        public HttpPostAction(string path, string url)
        {
            _path = path;
            _url = url;
        }

        public HttpPostAction(string url, object body)
        {
            _url = url;
            _body = JsonConvert.SerializeObject(body);
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            var testCase = testCaseContext.TestCase;
            if (!string.IsNullOrEmpty(_body))
            {
                Post(_url, _body, context, testCaseContext.Logger);
            }
            else if (testCase.Contains(_path))
            {
                ExecutePost(testCaseContext, context, _path);
            }
            else
            {
                foreach (var id in testCase.GetItemIds(_path))
                {
                    ExecutePost(testCaseContext, context, id);
                }
            }
        }

        private void ExecutePost(TestCaseContext testCaseContext, T context, string id)
        {
            Post(_url, testCaseContext.TestCase.GetContent(id), context, testCaseContext.Logger);
        }

        private static void Post(string url, string body, T context, ILog logger)
        {
            context.Content = context.HttpClient.PostEnsureSuccess(url, body, logger);
        }
    }
}
