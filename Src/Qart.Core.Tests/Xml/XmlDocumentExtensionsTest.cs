using NUnit.Framework;
using Qart.Core.Io;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Qart.Core.Xml;
using Qart.Core.DataStore;

namespace Qart.Core.Tests.Xml
{
    class XmlDocumentExtensionsTest
    {
        readonly TestSystem TestSystem = new TestSystem(new FileBasedDataStore(PathUtils.ResolveRelative(Path.Combine("TestData", "XmlDocumentExtensionsTests"))));

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
