using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class ContentProcessor : IContentProcessor
    {
        private IStreamTransformerResolver _streamTransformerResolver;

        public ContentProcessor(IStreamTransformerResolver streamTransformerResolver)
        {
            _streamTransformerResolver = streamTransformerResolver;
        }


        public System.IO.Stream Process(string content, Core.DataStore.IDataStore dataStore)
        {
            //[{"ref":"c:/dddd"},{"xslt" : "c:/dddd"}]
            Stream strm=null;
            foreach(var action in JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content))
            {
                var kvp = action.Single();
                var transformer = _streamTransformerResolver.ResolveTransformer(kvp.Key, kvp.Value);
                strm = transformer.Transform(strm);
            }
            return strm;
        }
    }
}
