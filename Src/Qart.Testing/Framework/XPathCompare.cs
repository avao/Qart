using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Qart.Testing.Framework
{
	public class XPathCompare
	{
		public bool ComparePathElements(IEnumerable<XElement> jsonElements, IEnumerable<XElement> xmlElements)
		{
			var isEqual = false;
			if (jsonElements.Count () > 1)
			{
				isEqual = IsElementAvailable(jsonElements, xmlElements);
			}
			else
			{
				isEqual = IsElementAvailable(jsonElements.FirstOrDefault (), xmlElements);
			}
			return isEqual;
		}

		private bool IsElementAvailable(XElement element, IEnumerable<XElement> xmlElements)
		{
			if (!element.Value.Equals (string.Empty))
			{
				return xmlElements.FirstOrDefault(y => y.Value == element.Value) != null;
			}
			return true;
		}

		private bool IsElementAvailable(IEnumerable<XElement> jsonElements, IEnumerable<XElement> xmlElements)
		{
			return jsonElements.Where (z => !z.Value.Equals (string.Empty)).
				All(x => xmlElements.FirstOrDefault (y => y.Value == x.Value) != null);
		}
	}
}

