using System.IO;
using Qart.Core.Io;
using Qart.Core.DataStore;
using Qart.Core.Text;

namespace Qart.Testing.StreamTransformers
{
    public class ConcatJsonArrayStreamTransformer : IStreamTransformer
    {
        public Stream Transform(Stream strm, Core.DataStore.IDataStore dataStore, object param)
        {
            string currentContent;
            using (var reader = new StreamReader(strm))
            {
                currentContent = reader.ReadToEnd();
            }
            currentContent = currentContent.TrimEnd();

            if (currentContent.EndsWith("]"))
            {
                currentContent = currentContent.Substring(0, currentContent.Length - 1);
            }

            var resultStream = new MemoryStream();

            resultStream.WriteUtf(currentContent);
            resultStream.WriteUtf(",");
            resultStream.WriteUtf(dataStore.GetContent((string)param).RightOf("["));

            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }
    }
}
