using System.Collections.Generic;
using System.Xml;
using Qart.Core.Xml;

namespace Qart.Core.Xml
{
	public abstract class XmlDocumentWrapper
	{
		public string OriginalContent { get; private set; }

		public XmlDocument Document { get; private set; }

		protected XmlDocumentWrapper(string content)
		{
			OriginalContent = content;
			Document = new XmlDocument();
			Document.LoadXml(content);
		}

		protected XmlDocumentWrapper(string content, IEnumerable<string> volatileProperties)
			: this(content)
		{
			Document.RemoveNodes (volatileProperties);
		}

		public override string ToString()
		{
			return XmlWriterUtils.ToXmlString(writer => Document.WriteTo(writer));
		}
	}
}