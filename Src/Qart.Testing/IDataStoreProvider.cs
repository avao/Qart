using Qart.Core.DataStore;

namespace Qart.Testing
{
    public interface IDataStoreProvider
    {
        IDataStore GetDataStore(string name);
        void RegisterDataStore(string name, IDataStore dataStore);
    }
}
