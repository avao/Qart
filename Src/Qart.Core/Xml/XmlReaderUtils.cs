using Qart.Core.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Qart.Core.Xml
{
    public static class XmlReaderUtils
    {
        public static void UsingXmlReader(string fileName, Action<XmlReader> action)
        {
            using (var stream = FileUtils.OpenFileStreamForReading(fileName))
            {
                stream.UsingXmlReader(action);
            }
        }

        public static void UsingXmlReader(this Stream stream, Action<XmlReader> action)
        {
            stream.UsingXmlReader(reader => { action(reader); return true; });
        }

        public static T UsingXmlReader<T>(this Stream stream, Func<XmlReader, T> action)
        {
            using (var reader = XmlReader.Create(stream))
            {
                return action(reader);
            }
        }
    }
}
