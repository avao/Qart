using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qart.Testing
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
            return _dataStore.GetReadStream(GetItemId(id));
        }

        public Stream GetWriteStream(string id)
        {
            return _dataStore.GetWriteStream(GetItemId(id));
        }

        public bool Contains(string id)
        {
            return _dataStore.Contains(GetItemId(id));
        }

        public IEnumerable<string> GetItemIds(string tag)
        {
            return _dataStore.GetItemIds(GetItemId(tag)).Select(_ => _.Substring(_tag.Length + 1));//TODO mixing tags and ids
        }

        private string GetItemId(string name)
        {
            return Path.Combine(_tag, name);
        }

    }

}
