using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Qart.Core.Xml
{
    public static class XmlDocumentExtensions
    {
        public static void RemoveNodes(this XmlDocument doc, IEnumerable<string> xpaths)
        {
            foreach (var xpath in xpaths)
            {
                XmlNode node = doc.SelectSingleNode(xpath);
                while(node != null) {
                    var attribute = node as XmlAttribute;
                    if (attribute != null)
                    {
                        attribute.OwnerElement.RemoveAttributeNode(attribute);
                    }
                    else
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                    node = doc.SelectSingleNode(xpath);
                }
            }
        }

        public static void OverrideWith(this XmlDocument lhs, XmlDocument rhs)
        {
            lhs.DocumentElement.Override(rhs.DocumentElement);
        }
    }
}
