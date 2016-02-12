using NUnit.Framework;
using Qart.Core.Io;
using Qart.Testing;
using Qart.Testing.FileBased;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Qart.Core.Xml;

namespace Qart.Core.Tests.Xml
{
    class XmlDocumentExtensionsTest
    {
        TestSystem TestSystem = new TestSystem(new DataStore(PathUtils.ResolveRelative(@"TestData\XmlDocumentExtensionsTests")));

        [TestCase("RepeatedElements")]
        public void Load_Merge_Assert(string testId)
        {
            var testCase = TestSystem.GetTestCase(testId);

            XmlDocument lhs = testCase.GetXmlDocument("lhs.xml");
            XmlDocument rhs = testCase.GetXmlDocument("rhs.xml");
            
            lhs.OverrideWith(rhs);

            testCase.AssertContentXml(lhs.OuterXml, "merged.xml", true);
        }

    }
}
