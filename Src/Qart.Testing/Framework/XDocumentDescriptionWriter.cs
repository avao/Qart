using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Qart.Testing.Framework
{
    public class XDocumentDescriptionWriter : IDescriptionWriter
    {
        private XDocument _document;
        private XElement _element;

        public XDocumentDescriptionWriter()
        {
            _document = new XDocument();
            _element = _document.Root;
        }

        public XDocumentDescriptionWriter(XElement element)
        {
            _element = element;
        }

        public void AddNote(string name, string value)
        {
            _element.Add(new XElement(name, value));
        }

        public IDescriptionWriter CreateNestedWriter(string scope)
        {
            var nestedElement = new XElement(scope);
            _element.Add(nestedElement);
            return new XDocumentDescriptionWriter(nestedElement);
        }

        public XDocument GetContent()
        {
            return _document;
        }
    }
}
