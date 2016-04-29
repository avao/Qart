using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Qart.Core.Text;

namespace Qart.Core.Xml
{
    public static class XmlNodeExtensions
    {
        public static void OrderElements(this XmlNode srcNode, Func<XmlElement, object> keySelector)
        {
            var allChidren = srcNode.ChildNodes.Cast<XmlNode>().ToList();
            foreach(var node in allChidren.OfType<XmlElement>().OrderBy(keySelector))
            {
                srcNode.RemoveChild(node);
                srcNode.AppendChild(node);
            }
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


                if (lhsElement.ChildNodes.Count == 0 && lhsElement.InnerText.IsXml() && rhs.ChildNodes.Count == 1)
                {//if lhs node is simple content and rhs node is complex then try parsing content as XML and apply overrides to it
                    var doc = new XmlDocument();
                    doc.LoadXml(lhsElement.InnerText);
                    Override(doc.DocumentElement, rhs.ChildNodes[0]);
                    lhsElement.InnerText = doc.OuterXml;
                    return;
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
