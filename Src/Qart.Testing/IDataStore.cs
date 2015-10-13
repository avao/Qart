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
        Stream GetStream(string itemId);

        void PutContent(string itemId, string content);
    }

    public static class DataStorageExtensions
    {
        public static string GetContent(this IDataStore dataStore, string itemId)
        {
            using(var stream = dataStore.GetStream(itemId))
            using(var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
