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
    public class TestCase
    {
        public TestSystem TestSystem { get; private set; }
        public string Id { get; private set; }

        internal TestCase(string id, TestSystem testSystem)
        {
            TestSystem = testSystem;
            Id = id;
        }

        public void AssertContentXml(string actualContent, string resultName, bool rebaseline)
        {
            string actualFormattedContent = XDocument.Parse(actualContent).ToString();

            string expectedContent = TestSystem.DataStorage.GetContent(GetItemId(resultName));

            string expectedFormattedContent = string.IsNullOrEmpty(expectedContent)? string.Empty : XDocument.Parse(expectedContent).ToString();

            if (expectedFormattedContent != actualFormattedContent)
            {
                if (rebaseline)
                {
                    TestSystem.DataStorage.PutContent(GetItemId(resultName), actualFormattedContent);
                }

                Assert.AreEqual(expectedFormattedContent, actualFormattedContent);
                Assert.Fail("Just in case...");
            }
        }

        public void AssertContent(string actualContent, string resultName, bool rebaseline)
        {
            string content = TestSystem.DataStorage.GetContent(GetItemId(resultName));
            if (content != actualContent)
            {
                if (rebaseline)
                {
                    TestSystem.DataStorage.PutContent(GetItemId(resultName), actualContent);
                }

                Assert.AreEqual(content, actualContent);
                Assert.Fail("Just in case...");
            }
        }


        private string GetItemId(string name)
        {
            return Path.Combine(Id, name);
        }
    }
}
