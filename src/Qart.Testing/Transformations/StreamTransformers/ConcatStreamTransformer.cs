﻿using System.IO;
using Qart.Core.Io;

namespace Qart.Testing.Transformations.StreamTransformers
{
    public class ConcatStreamTransformer : IStreamTransformer
    {
        public Stream Transform(Stream strm, Core.DataStore.IDataStore dataStore, object param)
        {
            var resultStream = new MemoryStream();
            resultStream.Append(strm);
            resultStream.Append(dataStore.GetReadStream((string)param));
            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }
    }
}
