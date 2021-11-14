using Qart.Core.DataStore;
using System.IO;

namespace Qart.Testing.Transformations.StreamTransformers
{
    public class RefStreamTransformer : IStreamTransformer
    {
        public Stream Transform(Stream strm, IDataStore dataStore, object reference)
        {
            return dataStore.GetReadStream((string)reference);
        }
    }
}
