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
        public IDataStore DataStorage { get; private set; }
        public string Id { get; private set; }

        internal TestCase(string id, TestSystem testSystem)
        {
            TestSystem = testSystem;
            DataStorage = new ScopedDataStore(testSystem.DataStorage, id);
            Id = id;
        }

        public Stream GetReadStream(string id)
        {
            if (DataStorage.Contains(id))
            {
                return DataStorage.GetReadStream(id);
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
            return DataStorage.GetWriteStream(id);
        }

        public bool Contains(string id)
        {
            return DataStorage.Contains(id) || DataStorage.Contains(GetItemRef(id));
        }


        private string GetItemRef(string name)
        {
            return name + ".ref"; //TODO reference as a concept
        }

        public IEnumerable<string> GetItemIds(string tag)
        {
            return DataStorage.GetItemIds(tag);
        }


        public IEnumerable<string> GetItemGroups(string group)
        {
            throw new NotImplementedException();
        }
    }

    public static class TestCaseExtensions
    {
        public static void UsingXmlReader(this TestCase testCase, string id, Action<XmlReader> action)
        {
            testCase.UsingXmlReader(id, reader => { action(reader); return true; });
        }

        public static T UsingXmlReader<T>(this TestCase testCase, string id, Func<XmlReader, T> action)
        {
            return testCase.UsingReadStream(id, stream => stream.UsingXmlReader(action));
        }

        public static void UsingXmlWriter(this TestCase testCase, string id, Action<XmlWriter> action)
        {
            testCase.UsingXmlWriter(id, writer => { action(writer); return true; });
        }

        public static T UsingXmlWriter<T>(this TestCase testCase, string id, Func<XmlWriter, T> action)
        {
            return testCase.UsingWriteStream(id, stream => stream.UsingXmlWriter(action, true));
        }

        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, Action<string, string> failAction)
        {
            string content = testCase.GetContent(resultName);
            if (content != actualContent)
            {
                failAction(actualContent, content);

                Assert.AreEqual(content, actualContent);
                Assert.Fail("Just in case...");
            }
        }

        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, Action<string, string> failAction, bool rebase)
        {
            testCase.AssertContent(actualContent, resultName, (actual, expected) =>
            {
                failAction(actual, expected);
                if (rebase)
                {
                    testCase.PutContent(resultName, actualContent);
                }
            });
        }

        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
        {
            testCase.AssertContent(actualContent, resultName, (actual, expected) => { }, rebaseline);
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
