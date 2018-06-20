using Newtonsoft.Json;
using Qart.Core.DataStore;

namespace Qart.Testing.Framework.Json
{
    public static class IDataStoreJsonExtensions
    {
        public static T GetObjectFromJson<T>(this IDataStore dataStore, string id)
        {
            return JsonConvert.DeserializeObject<T>(dataStore.GetContent(id));
        }
    }
}
