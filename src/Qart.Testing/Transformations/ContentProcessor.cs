using Newtonsoft.Json;
using Qart.Core.Activation;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qart.Testing.Transformations
{
    public class ContentProcessor : IContentProcessor
    {
        private readonly IObjectFactory<IStreamTransformer> _streamTransformerResolver;

        public ContentProcessor(IObjectFactory<IStreamTransformer> streamTransformerResolver)
        {
            _streamTransformerResolver = streamTransformerResolver;
        }

        public Stream Process(string content, Core.DataStore.IDataStore dataStore)
        {
            //[{"ref":"c:/dddd"},{"xslt" : "c:/dddd"}]
            Stream strm = null;
            foreach (var action in JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content))
            {
                var kvp = action.Single();
                var transformer = _streamTransformerResolver.Create(kvp.Key);
                strm = transformer.Transform(strm, dataStore, kvp.Value);
            }
            return strm;
        }
    }
}
