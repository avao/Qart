using Qart.Core.Io;
using System.IO;
using System.Xml.Schema;

namespace Qart.Core.Xsd
{
    public static class SchemaLoader
    {
        public static XmlSchema Load(Stream stream)
        {
            return XmlSchema.Read(stream, null);
        }

        public static XmlSchema Load(string path)
        {
            using (var stream = FileUtils.OpenFileStreamForReading(path))
            {
                return Load(stream);
            }
        }
    }
}
