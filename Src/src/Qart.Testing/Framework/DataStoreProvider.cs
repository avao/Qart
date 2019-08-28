using Qart.Core.DataStore;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public class DataStoreProvider : IDataStoreProvider
    {
        private readonly ConcurrentDictionary<string, IDataStore> _dataStores;

        public DataStoreProvider()
        {
            _dataStores = new ConcurrentDictionary<string, IDataStore>();
        }

        public IDataStore GetDataStore(string name)
        {
            IDataStore dataStore;
            _dataStores.TryGetValue(name, out dataStore);
            return dataStore;
        }

        public void RegisterDataStore(string name, IDataStore dataStore)
        {
            _dataStores.TryAdd(name, dataStore);
        }
    }
}
