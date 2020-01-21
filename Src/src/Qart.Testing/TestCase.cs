using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Core.DataStore;
using Qart.Core.Validation;
using Qart.Core.Xml;
using Qart.Testing.Diff;
using Qart.Testing.Framework.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Qart.Testing
{
    public class TestCase : IDataStore
    {
        public IDataStore DataStorage { get; }
        public IDataStore TmpDataStore { get; }
        public string Id { get; }

        private readonly IDataStoreProvider _dataStoreProvider;

        internal TestCase(string id, IDataStore dataStore, IDataStore tmpDataStore, IDataStoreProvider dataStoreProvider)
        {
            DataStorage = dataStore;
            TmpDataStore = tmpDataStore;
            Id = id;
            _dataStoreProvider = dataStoreProvider;
        }

        public Stream GetReadStream(string id)
        {
            var targetInfo = GetTargetInfo(id);
            return targetInfo.DataStore.GetReadStream(targetInfo.Id);
        }

        public Stream GetWriteStream(string id)
        {
            var targetInfo = GetTargetInfo(id);
            return targetInfo.DataStore.GetWriteStream(targetInfo.Id);
        }

        public bool Contains(string id)
        {
            var targetInfo = GetTargetInfo(id);
            return targetInfo.DataStore.Contains(targetInfo.Id);
        }

        public IEnumerable<string> GetItemIds(string tag)
        {
            return DataStorage.GetItemIds(tag);
        }

        public IEnumerable<string> GetItemGroups(string group)
        {
            throw new NotImplementedException();
        }

        private (IDataStore DataStore, string Id) GetTargetInfo(string id)
        {
            var ds = DataStorage;
            if (Uri.TryCreate(id, UriKind.Absolute, out Uri uri))
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

            return (ds, id);
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
                string message = MessageFormatter.FormatMessageNotEqual(actualContent, expectedContent, 200);
                throw new AssertException(message);
            }
        }

        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, Action<string, string> failAction, bool rebase)
        {
            testCase.AssertContent(actualContent, resultName, (actual, expected) =>
            {
                testCase.RebaseContentOrStoreTmp(resultName, actualContent, rebase);
                failAction(actual, expected);
            });
        }

        public static void AssertContentAsDiff(this TestCase testCase, JToken actual, string expectedName, string resultName, ITokenSelectorProvider idProvider, bool rebaseline)
        {
            AssertContentAsDiff(testCase, actual, testCase.GetObjectFromJson<JToken>(expectedName), resultName, idProvider, rebaseline);
        }

        public static void AssertContentAsDiff(this TestCase testCase, JToken actual, JToken expected, string resultName, ITokenSelectorProvider idProvider, bool rebaseline)
        {
            var diffs = JsonPatchCreator.Compare(expected, actual, idProvider);
            (var mismatches, var _) = testCase.CompareAndRebase(actual, expected, diffs, resultName, idProvider, rebaseline);
            if (mismatches.Count > 0)
            {
                throw new AssertException("Unexpected token changes:" + string.Join("\n", mismatches.Select(d => d.JsonPath)));
            }
        }

        public static (IReadOnlyCollection<DiffItem> mismatches, JToken expected) CompareAndRebase(this TestCase testCase, JToken actual, JToken @base, IEnumerable<DiffItem> actualDiffs, string resultName, ITokenSelectorProvider idProvider, bool rebaseline)
        {
            var expectedDiffs = testCase.GetObjectFromJson<IEnumerable<DiffItem>>(resultName) ?? Enumerable.Empty<DiffItem>();

            var expected = @base.DeepClone();
            expected.ApplyPatch(expectedDiffs);

            var mismatches = JsonPatchCreator.Compare(expected, actual, idProvider).ToList();
            if (mismatches.Count > 0)
            {
                testCase.RebaseContentOrStoreTmp(resultName, actualDiffs.ToIndentedJson(), rebaseline);
            }
            testCase.AddTmpItem("full_" + resultName, actual.ToIndentedJson());
            return (mismatches, expected);
        }

        public static void AssertContent(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
        {
            testCase.AssertContent(actualContent, resultName, (actual, expected) => { }, rebaseline);
        }

        public static void AssertContentJson(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
        {
            testCase.AssertContent(JsonConvert.DeserializeObject(actualContent).ToIndentedJson(), resultName, rebaseline);
        }

        public static void AssertContent(this TestCase testCase, XmlDocument actual, string resultName, bool rebaseline)
        {
            var actualContent = XmlWriterUtils.ToXmlString(writer => actual.WriteTo(writer));
            testCase.AssertContent(actualContent, resultName, rebaseline);
        }

        public static void AddTmpItem(this TestCase testCase, string itemId, string content)
        {
            testCase.TmpDataStore.PutContent(itemId, content);
        }

        public static void RebaseContentOrStoreTmp(this TestCase testCase, string itemId, string content, bool rebaseline)
        {
            var store = rebaseline ? testCase : testCase.TmpDataStore;
            store.PutContent(itemId, content);
        }
    }
}
