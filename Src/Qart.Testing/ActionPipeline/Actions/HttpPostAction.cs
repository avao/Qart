using Qart.Core.DataStore;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class HttpPostAction<T> : IPipelineAction<T>
        where T : IHttpContext
    {
        private string _path;
        private string _url;

        public HttpPostAction(string path, string url)
        {
            _path = path;
            _url = url;
        }

        public void Execute(TestCaseContext testCaseContext, T context)
        {
            var testCase = testCaseContext.TestCase;
            if (testCase.Contains(_path))
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
            context.Content = context.HttpClient.PostEnsureSuccess(_url, testCaseContext.TestCase.GetContent(id), testCaseContext.Logger);
        }
    }
}
