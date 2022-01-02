using Microsoft.Extensions.Logging;
using Qart.Core.Activation;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;
using Qart.Testing.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace Qart.Testing.ActionPipeline.Actions
{
    public class ExecuteAction : IPipelineAction
    {
        private readonly string _path;
        private readonly IObjectFactory<IPipelineAction> _pipelineActionFactory;

        public ExecuteAction(IObjectFactory<IPipelineAction> pipelineActionFactory, string path)
        {
            _pipelineActionFactory = pipelineActionFactory;
            _path = path;
        }

        public async Task ExecuteAsync(TestCaseContext testCaseContext)
        {
            var actionUrls = await testCaseContext.TestCase.GetObjectFromJsonAsync<string[]>(_path);
            var actionDefs = actionUrls.Select(DataStoreBasedTestStorage.ParseUrl).ToList();
            await testCaseContext.ExecuteActionsAsync(_pipelineActionFactory, actionDefs, testCaseContext.Options.GetDeferExceptions(), testCaseContext.Logger);
        }
    }
}
