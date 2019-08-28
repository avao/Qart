using Qart.Core.DataStore;
using System.IO;

namespace Qart.Testing
{
    public interface IStreamTransformer
    {
        Stream Transform(Stream strm, IDataStore dataStore, object param); 
    }
}
