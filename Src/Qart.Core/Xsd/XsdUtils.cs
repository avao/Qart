using Qart.Core.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
