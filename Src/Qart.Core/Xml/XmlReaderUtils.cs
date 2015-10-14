using Qart.Core.Io;
using System;
using System.Collections.Generic;
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
            using (var reader = XmlReader.Create(stream))
            {
                action(reader);
            }
        }
    }
}
