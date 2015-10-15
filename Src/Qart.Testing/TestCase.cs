using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;

namespace Qart.Testing
{
    public class TestCase : IDataStore
    {
        public TestSystem TestSystem { get; private set; }
        public string Id { get; private set; }

        internal TestCase(string id, TestSystem testSystem)
        {
            TestSystem = testSystem;
            Id = id;
        }

        
        public Stream GetReadStream(string id)
        {
            return TestSystem.DataStorage.GetReadStream(GetItemId(id));
        }

        public Stream GetWriteStream(string id)
        {
            return TestSystem.DataStorage.GetWriteStream(GetItemId(id));
        }

        private string GetItemId(string name)
        {
            return Path.Combine(Id, name);
        }
    }

    public static class TestCaseExtensions
    {
        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
        {
            string content = testCase.GetContent(resultName);
            if (content != actualContent)
            {
                if (rebaseline)
                {
                    testCase.PutContent(resultName, actualContent);
                }

                Assert.AreEqual(content, actualContent);
                Assert.Fail("Just in case...");
            }
        }

        public static void AssertContentXml(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
        {
            string actualFormattedContent = XDocument.Parse(actualContent).ToString();

            string expectedContent = testCase.GetContent(resultName);

            string expectedFormattedContent = string.IsNullOrEmpty(expectedContent) ? string.Empty : XDocument.Parse(expectedContent).ToString();

            if (expectedFormattedContent != actualFormattedContent)
            {
                if (rebaseline)
                {
                    testCase.PutContent(resultName, actualFormattedContent);
                }

                Assert.AreEqual(expectedFormattedContent, actualFormattedContent);
                Assert.Fail("Just in case...");
            }
        }

    }
}
