using Qart.Testing;
using Qart.Testing.ActionPipeline;
using System.Net.Http;

namespace Qart.Wheels.TestAutomation
{
    public class ItemProvider : IItemProvider
    {
        public bool TryGetItem<T>(string itemKey, out T item)
            where T : class
        {
            switch (itemKey)
            {
                case ItemKeys.HttpClient:
                    object httpClient = new HttpClient();
                    item = (T)httpClient;
                    return true;
            }

            item = default;
            return false;
        }
    }
}
