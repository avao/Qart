using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using System.Xml;
using Qart.Core.Xml;

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
            string itemId = GetItemId(id);
            if(DataStorage.Contains(itemId))
            {
                return DataStorage.GetReadStream(itemId);
            }
            else if (DataStorage.Contains(GetItemRef(id)))
            {
                string target = DataStorage.GetContent(GetItemRef(id));
                return GetReadStream(target);
            }
            return null;
        }

        public Stream GetWriteStream(string id)
        {
            return DataStorage.GetWriteStream(GetItemId(id));
        }

        public bool Contains(string id)
        {
            return DataStorage.Contains(GetItemId(id)) || DataStorage.Contains(GetItemRef(id));
        }

        private IDataStore DataStorage { get { return TestSystem.DataStorage; } }

        private string GetItemId(string name)
        {
            return Path.Combine(Id, name);
        }

        private string GetItemRef(string name)
        {
            return GetItemId(name) + ".ref";
        }


        public IEnumerable<string> GetItemIds(string tag)
        {
            return DataStorage.GetItemIds(tag);
        }

        public IEnumerable<string> Tags
        {
            get { return DataStorage.Tags; }
        }
    }

    public static class TestCaseExtensions
    {
        public static void UsingXmlReader(this TestCase testCase, string id, Action<XmlReader> action)
        {
            testCase.UsingReadStream(id, stream => stream.UsingXmlReader(action));
        }

        public static void UsingXmlWriter(this TestCase testCase, string id, Action<XmlWriter> action)
        {
            testCase.UsingWriteStream(id, stream => stream.UsingXmlWriter(action, true));
        }

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
