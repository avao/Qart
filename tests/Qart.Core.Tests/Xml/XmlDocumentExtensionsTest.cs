using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using Qart.Core.Xml;
using Qart.Testing.Framework;
using Qart.Testing.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Qart.Core.Tests.Xml
{
    class XmlDocumentExtensionsTest
    {
        readonly ITestStorage TestSystem = new DataStoreBasedTestStorage(new FileBasedDataStore(PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "XmlDocumentExtensionsTests"))), _ => true);

        [TestCase("Override/RepeatedElements"), Ignore("TODO: fix line ending")]
        public async Task Override(string testId)
        {
            var testCase = TestSystem.GetTestCase(testId);

            XmlDocument lhs = testCase.GetXmlDocument("Lhs.xml");
            XmlDocument rhs = testCase.GetXmlDocument("Rhs.xml");

            lhs.OverrideWith(rhs);

            await testCase.AssertContentAsync(lhs, "Merged.xml", true);
        }

        [TestCase("RemoveNodes/Element"), Ignore("TODO: fix line ending")]
        public async Task RemoveNodes(string testId)
        {
            var testCase = TestSystem.GetTestCase(testId);

            XmlDocument input = testCase.GetXmlDocument("input.xml");

            IEnumerable<string> xpaths = testCase.GetContent("xpaths.txt").Split(Environment.NewLine.ToCharArray());

            input.RemoveNodes(xpaths);

            await testCase.AssertContentAsync(input, "output.xml", true);
        }
    }
}
