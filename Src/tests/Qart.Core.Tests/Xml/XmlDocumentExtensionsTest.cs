using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using Qart.Core.Xml;
using Qart.Testing;
using Qart.Testing.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Qart.Core.Tests.Xml
{
    class XmlDocumentExtensionsTest
    {
        readonly ITestStorage TestSystem = new TestStorage(new FileBasedDataStore(PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "XmlDocumentExtensionsTests"))), _ => true, null, null);

        [TestCase("Override/RepeatedElements")]
        public void Override(string testId)
        {
            var testCase = TestSystem.GetTestCase(testId);

            XmlDocument lhs = testCase.GetXmlDocument("Lhs.xml");
            XmlDocument rhs = testCase.GetXmlDocument("Rhs.xml");

            lhs.OverrideWith(rhs);

            testCase.AssertContent(lhs, "Merged.xml", true);
        }

        [TestCase("RemoveNodes/Element")]
        public void RemoveNodes(string testId)
        {
            var testCase = TestSystem.GetTestCase(testId);

            XmlDocument input = testCase.GetXmlDocument("input.xml");

            IEnumerable<string> xpaths = testCase.GetContent("xpaths.txt").Split(Environment.NewLine.ToCharArray());

            input.RemoveNodes(xpaths);

            testCase.AssertContent(input, "output.xml", true);
        }
    }
}
