using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Common.Logging;
using Qart.Core.Xml;
using Qart.Testing;

namespace Qart.Testing.Framework
{
	public class XmlDocumentPreprocessor
	{
		private const string OverrideExtension = ".override.xpath";
		private readonly TestCase _testCase;
		private readonly string _itemId;
		private readonly ILog _logger;
		private readonly TagProcessor _tagProcessor;

		public XmlDocumentPreprocessor(TestCase testCase, string id, TagProcessor tagProcessor, ILog logger)
		{
			_testCase = testCase;
			_itemId = id;
			_tagProcessor = tagProcessor;
			_logger = logger;
		}

		IEnumerable<XPathOverride> GetXPathOverrides()
		{
			var overrideId = _itemId + OverrideExtension;
			if (!_testCase.Contains (overrideId))
			{
				_logger.TraceFormat("No override found: {0}", overrideId);
				return new XPathOverride[0];
			}
			return _testCase.GetObjectJson<IEnumerable<XPathOverride>>(overrideId);
		}

		private XmlDocument PerformXPathOverride(XmlDocument doc, XPathOverride xPathOverride)
		{
			XmlNode targetNode = doc.SelectSingleNode(xPathOverride.XPath);
			if (targetNode == null)
			{
				_logger.TraceFormat("Could not find XPath target: {0}", xPathOverride.XPath);
				return doc;
			}

			_logger.TraceFormat("Performing replacement for XPath target: {0}, value {1}", xPathOverride.XPath, xPathOverride.Value);
			_logger.TraceFormat("Content value of the node: {0}", targetNode.InnerText);
			string value = _tagProcessor.Process(xPathOverride.Value);
			_logger.TraceFormat("After tag processing '{0}'", value);
			targetNode.InnerText = value;
			return doc;
		}

		private XmlDocument PerformXPathOverrideWithExpansion(XmlDocument doc, XPathOverride xPathOverride)
		{
			// hack for handling XML contained within XML as CData.
			_logger.DebugFormat ("Found element that requires expansion: '{0}'", 
				xPathOverride.OwningElementThatRequiresExpansion);
			XmlNode parentNode = doc.SelectSingleNode(xPathOverride.OwningElementThatRequiresExpansion);
			if (parentNode == null)
			{
				_logger.TraceFormat("Could not find XPath target: '{0}'", xPathOverride.OwningElementThatRequiresExpansion);
				return doc;
			}

			if (!parentNode.InnerXml.StartsWith("<?xml") || parentNode.ChildNodes[0].HasChildNodes)
			{
				_logger.TraceFormat("Element does not appear to be embedded XML, so ignoring it: '{0}'",
					xPathOverride.OwningElementThatRequiresExpansion);
				return doc;
			}

			var xdoc = new XmlDocument();
			xdoc.LoadXml(parentNode.InnerText);
			parentNode.InnerText = PerformXPathOverride(xdoc, xPathOverride).OuterXml;
			return doc;
		}

		XmlDocument PerformAllXPathOverrides(XmlDocument doc)
		{
			foreach (var xPathOverride in GetXPathOverrides())
			{
				bool requiresExpansion = !string.IsNullOrWhiteSpace(xPathOverride.OwningElementThatRequiresExpansion);
				if (requiresExpansion)
					doc = PerformXPathOverrideWithExpansion(doc, xPathOverride);
				else
					doc = PerformXPathOverride(doc, xPathOverride);
			}
			return doc;
		}

		private XmlDocument PerformAllXPathExcludes(XmlDocument doc)
		{
			var xPathsToExclude = _testCase.GetXPathsToExclude(_itemId).ToList();
			if (xPathsToExclude.Count == 0)
			{
				_logger.Trace("No XPaths to exclude found.");
				return doc;
			}
			doc.RemoveNodes(xPathsToExclude);
			return doc;
		}

		public string Preprocess(string xmlDocAsString)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xmlDocAsString);
			doc = PerformAllXPathOverrides(doc);
			doc = PerformAllXPathExcludes(doc);
			return doc.OuterXml;
		}
	}
}

