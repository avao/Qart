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
                foreach (XmlNode node in doc.SelectNodes(xpath))
                {
                    var attribute = node as XmlAttribute;
                    if (attribute != null)
                    {
                        attribute.OwnerElement.RemoveAttributeNode(attribute);
                    }
                    else
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }
        }

        public static void OverrideWith(this XmlDocument lhs, XmlDocument rhs)
        {
            lhs.DocumentElement.Override(rhs.DocumentElement);
        }

        public static void Override(this XmlNode lhs, XmlNode rhs)
        {
            var lhsElement = lhs as XmlElement;
            if (lhsElement != null)
            {
                var rhsElement = rhs as XmlElement;

                foreach (XmlAttribute attribute in rhsElement.Attributes)
                {
                    var lhsAttribute = lhs.Attributes.Cast<XmlAttribute>().FirstOrDefault(_ => _.Name == attribute.Name);
                    if (lhsAttribute == null)
                    {
                        lhsElement.SetAttribute(attribute.Name, attribute.NamespaceURI, attribute.Value);
                    }
                    else
                    {
                        lhsAttribute.Value = attribute.Value;
                    }
                }


                var currentLhsNode = lhs.ChildNodes.Cast<XmlNode>().FirstOrDefault();
                foreach (XmlNode rhsNode in rhs.ChildNodes)
                {
                    var comment = rhsNode as XmlComment;
                    if (comment != null)
                        continue;

                    var content = rhsNode as XmlText;
                    if (content != null)
                    {
                        lhsElement.InnerText = content.Value;
                        continue;
                    }

                    if (currentLhsNode != null && currentLhsNode.LocalName != rhsNode.LocalName)
                    {
                        currentLhsNode = FindNextSibling(currentLhsNode, rhsNode.LocalName);
                    }

                    if (currentLhsNode == null)
                    {
                        //TODO it is only possible to override existing nodes
                        throw new Exception(string.Format("Could not find a node with name [{0}]", rhsNode.LocalName));
                    }

                    Override(currentLhsNode, rhsNode);

                    currentLhsNode = currentLhsNode.NextSibling;
                }


                //TODO content
                /*if (rhs.ChildNodes.Count == 0 && rhs.ChildNodes.Count == 0)
                    lhs.Value = rhs.Value;*/
            }
        }

        private static XmlNode FindNextSibling(XmlNode currentNode, string localName)
        {
            if (currentNode == null)
                return null;

            do
            {
                currentNode = currentNode.NextSibling;
            }
            while (currentNode != null && currentNode.LocalName != localName);

            return currentNode;
        }
    }
}
