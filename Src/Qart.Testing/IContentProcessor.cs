using Qart.Core.DataStore;
using System.IO;

namespace Qart.Testing
{
    public interface IContentProcessor
    {
        Stream Process(string content, IDataStore dataStore);
    }
}
