using System.IO;
using Qart.Core.Io;

namespace Qart.Testing.StreamTransformers
{
    public class ConcatStreamTransformer : IStreamTransformer
    {
        public Stream Transform(Stream strm, Core.DataStore.IDataStore dataStore, object param)
        {
            MemoryStream resultStream = new MemoryStream();
            resultStream.Append(dataStore.GetReadStream((string)param));
            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }
    }
}
