using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Xml;
using Qart.Testing.Diff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Qart.Testing
{
    public class TestCase : IDataStore
    {
        public ITestStorage TestStorage { get; private set; }
        public IDataStore DataStorage { get; private set; }
        public string Id { get; private set; }

        private readonly IDataStoreProvider _dataStoreProvider;

        internal TestCase(string id, ITestStorage testStorage, IDataStore dataStore, IDataStoreProvider dataStoreProvider)
        {
            TestStorage = testStorage;
            DataStorage = dataStore;
            Id = id;
            _dataStoreProvider = dataStoreProvider;
        }

        public Stream GetReadStream(string id)
        {
            var targetInfo = GetTargetInfo(id);
            return targetInfo.Item1.GetReadStream(targetInfo.Item2);
        }

        public Stream GetWriteStream(string id)
        {
            var targetInfo = GetTargetInfo(id);
            return targetInfo.Item1.GetWriteStream(targetInfo.Item2);
        }

        public bool Contains(string id)
        {
            var targetInfo = GetTargetInfo(id);
            return targetInfo.Item1.Contains(targetInfo.Item2);
        }

        public IEnumerable<string> GetItemIds(string tag)
        {
            return DataStorage.GetItemIds(tag);
        }

        public IEnumerable<string> GetItemGroups(string group)
        {
            throw new NotImplementedException();
        }

        private Tuple<IDataStore, string> GetTargetInfo(string id)
        {
            var ds = DataStorage;
            Uri uri;
            if (Uri.TryCreate(id, UriKind.Absolute, out uri))
            {
                if (uri.Scheme == "ds")
                {
                    ds = _dataStoreProvider.GetDataStore(uri.Host);
                    id = uri.PathAndQuery.Substring(1);
                }
                else if (uri.Scheme == "file")
                {
                }
                else
                {
                    throw new NotSupportedException(string.Format("Not supported uri scheme [{0}]", uri.Scheme));
                }
            }

            return new Tuple<IDataStore, string>(ds, id);
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

        public static T GetObjectJson<T>(this TestCase testCase, string id)
        {
            return JsonConvert.DeserializeObject<T>(testCase.GetContent(id));
        }

        public static XmlDocument GetXmlDocument(this TestCase testCase, string id)
        {
            var xmlDocument = new XmlDocument();
            testCase.UsingXmlReader(id, xmlDocument.Load);

            string overrideId = id + ".override";
            if (testCase.Contains(overrideId))
            {
                xmlDocument.OverrideWith(testCase.GetXmlDocument(overrideId));
            }
            return xmlDocument;
        }

        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, Action<string, string> failAction)
        {
            string expectedContent = testCase.GetContent(resultName);
            if (expectedContent != actualContent)
            {
                failAction(actualContent, expectedContent);

                Assert.That(actualContent, Is.EqualTo(expectedContent));
                Assert.Fail("Just in case...");
            }
        }

        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, Action<string, string> failAction, bool rebase)
        {
            testCase.AssertContent(actualContent, resultName, (actual, expected) =>
            {
                if (rebase)
                {
                    testCase.PutContent(resultName, actualContent);
                }
                failAction(actual, expected);
            });
        }



        public static void AssertContentAsDiff(this TestCase testCase, JToken actual, string expectedName, string resultName, ITokenSelectorProvider idProvider, bool rebaseline)
        {
            AssertContentAsDiff(testCase, actual, JToken.Parse(testCase.GetRequiredContent(expectedName)), resultName, idProvider, rebaseline);
        }

        public static void AssertContentAsDiff(this TestCase testCase, JToken actual, JToken expected, string resultName, ITokenSelectorProvider idProvider, bool rebaseline)
        {
            var expectedContent = testCase.GetContent(resultName);
            var diffs = JsonPatchCreator.Compare(expected, actual, idProvider);
            testCase.AssertDiffs(diffs, resultName, ReportDiffs, rebaseline);
        }

        public static void AssertDiffs(this TestCase testCase, IEnumerable<DiffItem> diffs, string resultName, Action<IEnumerable<DiffItem>> reportFunc, bool rebaseline)
        {
            string expectedContent = testCase.GetContent(resultName);
            var expectedDiffs = JsonConvert.DeserializeObject<IEnumerable<DiffItem>>(expectedContent);

            var diffDiffs = Compare(diffs, expectedDiffs).ToList();
            if (diffDiffs.Count > 0)
            {
                var diffContent = SerialiseIndented(diffs);
                if (rebaseline)
                {
                    testCase.PutContent(resultName, diffContent);
                }

                reportFunc(diffDiffs);
            }
        }

        private static void ReportDiffs(IEnumerable<DiffItem> diffs)
        {
            throw new Exception("Unexpected token changes:" + string.Join("\n", diffs.Select(d => d.JsonPath)));
        }

        private static IEnumerable<DiffItem> Compare(IEnumerable<DiffItem> actualDiffs, IEnumerable<DiffItem> expectedDiffs)
        {
            return JsonPatchCreator.Compare(JsonConvert.SerializeObject(actualDiffs), JsonConvert.SerializeObject(expectedDiffs), new PropertyBasedTokenSelectorProvider("path"));
        }

        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
        {
            testCase.AssertContent(actualContent, resultName, (actual, expected) => { }, rebaseline);
        }

        public static void AssertContentJson(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
        {
            testCase.AssertContent(SerialiseIndented(JsonConvert.DeserializeObject(actualContent)), resultName, rebaseline);
        }

        public static void AssertContent(this TestCase testCase, XmlDocument actual, string resultName, bool rebaseline)
        {
            var actualContent = XmlWriterUtils.ToXmlString(writer => actual.WriteTo(writer));
            testCase.AssertContent(actualContent, resultName, rebaseline);
        }

        private static string SerialiseIndented(object o)
        {
            return JsonConvert.SerializeObject(o, new JsonSerializerSettings { Formatting = Newtonsoft.Json.Formatting.Indented });
        }
    }
}
