using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qart.Core.DataStore
{
    public class ScopedDataStore : IDataStore
    {
        private readonly IDataStore _dataStore;
        private readonly string _tag;
        public ScopedDataStore(IDataStore dataStore, string tag)
        {
            _dataStore = dataStore;
            _tag = tag;
        }

        public Stream GetReadStream(string id)
        {
            return _dataStore.GetReadStream(GetScopedId(id));
        }

        public Stream GetWriteStream(string id)
        {
            return _dataStore.GetWriteStream(GetScopedId(id));
        }

        public bool Contains(string id)
        {
            return _dataStore.Contains(GetScopedId(id));
        }

        public IEnumerable<string> GetItemIds(string tag)
        {
            return _dataStore.GetItemIds(GetScopedId(tag)).Select(_ => _.Substring(_tag.Length + 1));//TODO mixing tags and ids
        }

        private string GetScopedId(string name)
        {
            if(!Path.IsPathRooted(name))
                return Path.Combine(_tag, name);
            return name;
        }

        public IEnumerable<string> GetItemGroups(string group)
        {
            return _dataStore.GetItemGroups(GetScopedId(group));
        }
    }
}
