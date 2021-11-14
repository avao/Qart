using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using Qart.Testing.ActionPipeline;
using Qart.Testing.Context;
using Qart.Testing.Execution;
using Qart.Testing.Storage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qart.CyberTester.Tests
{
    public class TestCases
    {
        public static IReadOnlyCollection<string> GetTestIds()
        {
            return new DataStoreBasedTestStorage(new FileBasedDataStore(GetTestsPath()), _ => _.Contains(".test"))
                .GetTestCaseIds();
        }

        [TestCaseSource(nameof(GetTestIds))]
        public async Task TestCase(string testId)
        {
            var path = Path.Combine(GetTestsPath(), testId);
            var serviceProvider = Program.CreateServiceProvider(path, null, services => services.AddSingleton<IItemProvider, ItemProvider>());
            var results = await serviceProvider.GetRequiredService<Testing.CyberTester>().RunTestsAsync(serviceProvider.GetService<ITestSession>(), new Dictionary<string, string>());
            var exception = results.Single().Exception;
            if(exception != null)
                throw exception;
        }

        private static string GetTestsPath()
        {
            return PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "TestCases"));
        }

        private class ItemProvider : IItemProvider
        {
            private static readonly object _httpClient = new HttpClient();

            public bool TryGetItem<T>(string itemKey, out T item)
            {
                (item, var result) = itemKey switch
                {
                    ItemKeys.HttpClient => ((T)_httpClient, true),
                    _ => (default, false)
                };
                return result;
            }
        }
    }
}
