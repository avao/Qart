using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Qart.Testing.Framework
{
    public class XDocumentDescriptionWriter : IDescriptionWriter
    {
        private readonly XDocument _document;
        private readonly XElement _element;
        private readonly ILogger _logger;

        public XDocumentDescriptionWriter(ILogger logger)
        {
            _document = new XDocument(new XElement("Description"));
            _element = _document.Root;
            _logger = logger;
        }

        public XDocumentDescriptionWriter(XElement element, ILogger logger)
        {
            _element = element;
            _logger = logger;
        }

        public void AddNote(string name, string value)
        {
            _element.Add(new XElement(name, value));
            _logger?.LogDebug("Adding note {note}", new { name, value });
        }

        public IDescriptionWriter CreateNestedWriter(string scope)
        {
            var nestedElement = new XElement(scope);
            _element.Add(nestedElement);
            return new XDocumentDescriptionWriter(nestedElement, _logger);
        }

        public XDocument GetContent()
        {
            return _document;
        }
    }
}
