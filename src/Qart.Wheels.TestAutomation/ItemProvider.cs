using Qart.Testing.ActionPipeline;
using Qart.Testing.Context;
using System.Net.Http;

namespace Qart.Wheels.TestAutomation
{
    public class ItemProvider : IItemProvider
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
