using Newtonsoft.Json;
using Qart.Core.DataStore;
using System.Threading.Tasks;

namespace Qart.Testing.Framework.Json
{
    public static class IDataStoreJsonExtensions
    {
        public static T GetObjectFromJson<T>(this IDataStore dataStore, string id)
        {
            var content = dataStore.GetContent(id);
            return content != null
                ? JsonConvert.DeserializeObject<T>(content)
                : default;
        }

        public static async Task<T> GetObjectFromJsonAsync<T>(this IDataStore dataStore, string id)
        {
            var content = await dataStore.GetContentAsync(id);
            return content != null
                ? JsonConvert.DeserializeObject<T>(content)
                : default;
        }
    }
}
