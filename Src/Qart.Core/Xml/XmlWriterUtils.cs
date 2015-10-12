using Qart.Core.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Qart.Core.Xml
{
    public static class XmlWriterUtils
    {
        public static string ToXmlString(Action<XmlWriter> action)
        {
            return ToXmlString(action, true);
        }
        public static string ToXmlString(Action<XmlWriter> action, bool indent)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = indent }))
            {
                action(writer);
            }
            return sb.ToString();
        }

        public static void ToXmlFile(string fileName, Action<XmlWriter> action, bool indent)
        {
            FileUtils.EnsureCanBeWritten(fileName);
            using (var stream = FileUtils.OpenFileStreamForWriting(fileName))
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = indent }))
            {
                action(writer);
            }
        }
    }
}
