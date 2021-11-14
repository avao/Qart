using Qart.Core.DataStore;
using System.Collections.Concurrent;

namespace Qart.Testing.Storage
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
            _dataStores.TryGetValue(name, out IDataStore dataStore);
            return dataStore;
        }

        public void RegisterDataStore(string name, IDataStore dataStore)
        {
            _dataStores.TryAdd(name, dataStore);
        }
    }
}
