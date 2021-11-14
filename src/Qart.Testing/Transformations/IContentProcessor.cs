using Qart.Core.DataStore;
using System.IO;

namespace Qart.Testing.Transformations
{
    public interface IContentProcessor
    {
        Stream Process(string content, IDataStore dataStore);
    }
}
