using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Qart.Core.Xsd
{
    public static class XmlSchemaSetExtensions
    {
        public static void AddFromAssembly(this XmlSchemaSet xmlSchemaSet, Assembly assembly)
        {
            foreach (var name in assembly.GetManifestResourceNames().Where(n => n.EndsWith(".xsd")))
            {
                xmlSchemaSet.AddFromAssembly(assembly, name);
            }
        }

        public static void AddFromAssembly(this XmlSchemaSet xmlSchemaSet, Assembly assembly, string resourceName)
        {
            xmlSchemaSet.Add(XmlSchemaLoader.Load(assembly.GetManifestResourceStream(resourceName)));
        }

        public static void ValidateXml(this XmlSchemaSet xmlSchemaSet, string content)
        {
            xmlSchemaSet.ValidateXml(content, (object sender, ValidationEventArgs e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                    throw e.Exception;
            });
        }

        public static void ValidateXml(this XmlSchemaSet xmlSchemaSet, string content, ValidationEventHandler validateionEventHandler)
        {
            var settings = new XmlReaderSettings
            {
                Schemas = xmlSchemaSet,
                ValidationType = ValidationType.Schema
            };
            settings.ValidationEventHandler += validateionEventHandler;

            using var stringReader = new StringReader(content);
            using XmlReader reader = XmlReader.Create(stringReader, settings);
            while (reader.Read())
            {
            }
        }
    }
}
