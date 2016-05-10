﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qart.Testing
{
    public class ContentProcessor : IContentProcessor
    {
        private readonly IStreamTransformerResolver _streamTransformerResolver;

        public ContentProcessor(IStreamTransformerResolver streamTransformerResolver)
        {
            _streamTransformerResolver = streamTransformerResolver;
        }

        public Stream Process(string content, Core.DataStore.IDataStore dataStore)
        {
            //[{"ref":"c:/dddd"},{"xslt" : "c:/dddd"}]
            Stream strm=null;
            foreach(var action in JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content))
            {
                var kvp = action.Single();
                var transformer = _streamTransformerResolver.ResolveTransformer(kvp.Key);
                strm = transformer.Transform(strm, dataStore, kvp.Value);
            }
            return strm;
        }
    }
}
