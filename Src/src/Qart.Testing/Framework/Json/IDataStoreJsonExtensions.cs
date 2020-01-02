using Newtonsoft.Json;
using Qart.Core.DataStore;

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
    }
}
