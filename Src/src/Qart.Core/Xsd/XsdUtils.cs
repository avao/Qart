using Qart.Core.Io;
using System.IO;
using System.Xml;
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
            using var stream = FileUtils.OpenFileStreamForReading(path);
            using var reader = XmlReader.Create(stream, null, path);

            return XmlSchema.Read(reader, null);
        }
    }
}
