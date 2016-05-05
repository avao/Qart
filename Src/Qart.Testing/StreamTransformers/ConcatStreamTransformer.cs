using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qart.Core.Io;

namespace Qart.Testing.StreamTransformers
{
    public class ConcatStreamTransformer : IStreamTransformer
    {
        public System.IO.Stream Transform(System.IO.Stream strm, Core.DataStore.IDataStore dataStore, object param)
        {
            MemoryStream resultStream = new MemoryStream();
            resultStream.Append(dataStore.GetReadStream((string)param));
            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }
    }
}
