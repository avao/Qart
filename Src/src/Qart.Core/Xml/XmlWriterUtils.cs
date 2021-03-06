﻿using Qart.Core.Io;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Qart.Core.Xml
{
    public static class XmlWriterUtils
    {
        private class CustomEncodingStringWriter : StringWriter
        {
            private readonly Encoding _encoding;
            public CustomEncodingStringWriter(StringBuilder stringBuilder, Encoding encoding)
                : base(stringBuilder)
            {
                _encoding = encoding;
            }
            public override Encoding Encoding => _encoding;
        }

        public static string ToXmlString(Action<XmlWriter> action)
        {
            return ToXmlString(action, true);
        }

        public static string ToXmlString(Action<XmlWriter> action, bool indent)
        {
            var sb = new StringBuilder();
            using (var stringWriter = new CustomEncodingStringWriter(sb, null))
            using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = indent }))
            {
                action(writer);
            }
            return sb.ToString();
        }

        public static void ToXmlFile(string fileName, Action<XmlWriter> action, bool indent)
        {
            FileUtils.EnsureCanBeWritten(fileName);
            using (var stream = FileUtils.OpenFileStreamForWriting(fileName))
            {
                stream.UsingXmlWriter(action, indent);
            }
        }

        public static void UsingXmlWriter(this Stream stream, Action<XmlWriter> action, bool indent)
        {
            stream.UsingXmlWriter(writer => { action(writer); return true; }, indent);
        }

        public static T UsingXmlWriter<T>(this Stream stream, Func<XmlWriter, T> action, bool indent)
        {
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = indent }))
            {
                return action(writer);
            }
        }
    }
}
