using Qart.Core.DataStore;
using System.IO;

namespace Qart.Testing.Transformations
{
    public interface IStreamTransformer
    {
        Stream Transform(Stream strm, IDataStore dataStore, object param);
    }
}
