using Qart.Core.DataStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.StreamTransformers
{
    public class RefStreamTransformer : IStreamTransformer
    {
        public Stream Transform(Stream strm, IDataStore dataStore, object reference)
        {
            return dataStore.GetReadStream((string)reference);
        }
    }
}
