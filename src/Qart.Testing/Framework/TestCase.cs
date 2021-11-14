using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Core.DataStore;
using Qart.Core.Io;
using Qart.Core.Validation;
using Qart.Core.Xml;
using Qart.Testing.Diff;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Json;
using Qart.Testing.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Qart.Testing.Framework
{
    public class TestCase : IDataStore
    {
        public IDataStore DataStorage { get; }
        public IDataStore TmpDataStore { get; }
        public string Id { get; }

        public IReadOnlyCollection<string> Tags { get; }
        public IReadOnlyCollection<ResolvableItemDescription> Actions { get; }

        private readonly IDataStoreProvider _dataStoreProvider;

        internal TestCase(string id, IReadOnlyCollection<string> tags, IReadOnlyCollection<ResolvableItemDescription> actions, IDataStore dataStore, IDataStore tmpDataStore, IDataStoreProvider dataStoreProvider)
        {
            DataStorage = dataStore;
            TmpDataStore = tmpDataStore;
            Id = id;
            _dataStoreProvider = dataStoreProvider;
            Tags = tags;
            Actions = actions;
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

        public static async Task AssertContentAsync(this TestCase testCase, string actualContent, string resultName, Action<string, string> failAction)
        {
            string expectedContent = await testCase.GetContentAsync(resultName);
            if (expectedContent != actualContent)
            {
                failAction(actualContent, expectedContent);
                string message = MessageFormatter.FormatMessageNotEqual(actualContent, expectedContent, 200);
                throw new AssertException(message);
            }
        }

        public static async Task AssertContentAsync(this TestCase testCase, string actualContent, string resultName, Action<string, string> failAction, bool rebase)
        {
            await testCase.AssertContentAsync(actualContent, resultName, async (actual, expected) =>
            {
                await testCase.RebaseContentOrStoreTmpAsync(resultName, actualContent, rebase);
                failAction(actual, expected);
            });
        }

        public static Task AssertContentAsDiffAsync(this TestCase testCase, JToken actual, string expectedName, string resultName, ITokenSelectorProvider idProvider, bool rebaseline)
        {
            return testCase.AssertContentAsDiffAsync(actual, testCase.GetObjectFromJson<JToken>(expectedName), resultName, idProvider, rebaseline);
        }

        public static async Task AssertContentAsDiffAsync(this TestCase testCase, JToken actual, JToken expected, string resultName, ITokenSelectorProvider idProvider, bool rebaseline)
        {
            var diffs = JsonPatchCreator.Compare(expected, actual, idProvider);
            (var mismatches, var _) = await testCase.CompareAndRebaseAsync(actual, expected, diffs, resultName, idProvider, rebaseline);
            if (mismatches.Count > 0)
            {
                throw new AssertException("Unexpected token changes:" + string.Join("\n", mismatches.Select(d => d.JsonPath)));
            }
        }

        public static async Task<(IReadOnlyCollection<DiffItem> mismatches, JToken expected)> CompareAndRebaseAsync(this TestCase testCase, JToken actual, JToken @base, IEnumerable<DiffItem> actualDiffs, string resultName, ITokenSelectorProvider idProvider, bool rebaseline)
        {
            var expectedDiffs = await testCase.GetObjectFromJsonAsync<IEnumerable<DiffItem>>(resultName) ?? Enumerable.Empty<DiffItem>();

            var expected = @base.DeepClone();

            try
            {
                expected.ApplyPatch(expectedDiffs);
            }
            catch (Exception)
            {
                await testCase.RebaseContentOrStoreTmpAsync(resultName, actualDiffs.ToIndentedJson(), rebaseline);
                throw;
            }

            var mismatches = JsonPatchCreator.Compare(expected, actual, idProvider).ToList();
            if (mismatches.Count > 0)
            {
                await testCase.RebaseContentOrStoreTmpAsync(resultName, actualDiffs.ToIndentedJson(), rebaseline);
            }
            await testCase.AddTmpItemAsync("full_" + resultName, actual.ToIndentedJson());
            return (mismatches, expected);
        }

        public static async Task AssertContentAsync(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
            => await testCase.AssertContentAsync(actualContent, resultName, (actual, expected) => { }, rebaseline);


        public static async Task AssertContentJsonAsync(this TestCase testCase, string actualContent, string resultName, bool rebaseline)
        {
            await testCase.AssertContentAsync(JsonConvert.DeserializeObject(actualContent).ToIndentedJson(), resultName, rebaseline);
        }

        public static async Task AssertContentAsync(this TestCase testCase, XmlDocument actual, string resultName, bool rebaseline)
        {
            var actualContent = XmlWriterUtils.ToXmlString(writer => actual.WriteTo(writer));
            await testCase.AssertContentAsync(actualContent, resultName, rebaseline);
        }

        public static async Task AddTmpItemAsync(this TestCase testCase, string itemId, string content)
        {
            await testCase.TmpDataStore.PutContentAsync(itemId, content);
        }

        public static async Task RebaseContentOrStoreTmpAsync(this TestCase testCase, string itemId, string content, bool rebaseline)
        {
            (IDataStore Store, string Id) spec = rebaseline
                ? (testCase, itemId)
                : (testCase.TmpDataStore, PathUtils.ReplaceInvalidChars(itemId));

            await spec.Store.PutContentAsync(spec.Id, content);
        }
    }
}
