using NUnit.Framework;
using Qart.Core.Io;
using Qart.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Qart.Core.Xml;
using Qart.Core.DataStore;

namespace Qart.Core.Tests.Xml
{
    class XmlDocumentExtensionsTest
    {
        TestSystem TestSystem = new TestSystem(new FileBasedDataStore(PathUtils.ResolveRelative(@"TestData\XmlDocumentExtensionsTests")));

        [TestCase(@"override\RepeatedElements")]
        public void Override(string testId)
        {
            var testCase = TestSystem.GetTestCase(testId);

            XmlDocument lhs = testCase.GetXmlDocument("lhs.xml");
            XmlDocument rhs = testCase.GetXmlDocument("rhs.xml");
            
            lhs.OverrideWith(rhs);

            testCase.AssertContent(lhs, "merged.xml", true);
        }

        [TestCase(@"removeNodes/Element")]
        public void RemoveNodes(string testId)
        {
            var testCase = TestSystem.GetTestCase(testId);

            XmlDocument input = testCase.GetXmlDocument("input.xml");
            IEnumerable<string> xpaths = testCase.GetContent("xpaths.txt").Split('\n');

            input.RemoveNodes(xpaths);

            testCase.AssertContent(input, "output.xml", true);
        }
    }
}
