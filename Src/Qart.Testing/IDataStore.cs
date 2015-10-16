using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public interface IDataStore
    {
        Stream GetReadStream(string itemId);

        Stream GetWriteStream(string itemId);

        bool Contains(string itemId);
    }

    public static class DataStorageExtensions
    {
        /// <summary>
        /// Retrieves content of the requested item. Returns null if an item is missing.
        /// </summary>
        /// <param name="dataStore"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetContent(this IDataStore dataStore, string itemId)
        {
            if (!dataStore.Contains(itemId))
                return null;
            using(var stream = dataStore.GetReadStream(itemId))
            using(var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static void PutContent(this IDataStore dataStore, string itemId, string content)
        {
            using (var stream = dataStore.GetWriteStream(itemId))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(content);
            }
        }

        public static void UsingReadStream(this IDataStore dataStore, string id, Action<Stream> action)
        {
            using (var stream = dataStore.GetReadStream(id))
            {
                action(stream);
            }
        }

        public static void UsingWriteStream(this IDataStore dataStore, string id, Action<Stream> action)
        {
            using (var stream = dataStore.GetWriteStream(id))
            {
                action(stream);
            }
        }

    }
}
