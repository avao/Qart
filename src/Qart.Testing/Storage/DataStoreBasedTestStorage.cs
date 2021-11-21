using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qart.Core.Collections;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Qart.Core.Validation;
using Qart.Testing.Framework;
using Qart.Testing.Transformations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Qart.Testing.Storage
{
    public class DataStoreBasedTestStorage : ITestStorage
    {
        public IDataStore DataStorage { get; }

        public IContentProcessor ContentProcessor { get; }

        private readonly Func<IDataStore, bool> _isTestCasePredicate;
        private readonly IDataStoreProvider _dataStoreProvider;
        private readonly ILogger _logger;

        public DataStoreBasedTestStorage(IDataStore dataStorage, Func<IDataStore, bool> isTestCasePredicate, IContentProcessor processor = null, IDataStoreProvider dataStoreProvider = null, ILogger<DataStoreBasedTestStorage> logger = null)
        {
            _isTestCasePredicate = isTestCasePredicate;
            ContentProcessor = processor;
            DataStorage = dataStorage;
            _dataStoreProvider = dataStoreProvider;

            _logger = logger;
        }

        public TestCase GetTestCase(string id)
        {
            _logger?.LogTrace("Creating TestCase {id}", id);

            var scopedTestCaseDataStore = new ScopedDataStore(DataStorage, id);
            var testCaseDataStore = new ExtendedDataStore(scopedTestCaseDataStore, (ds, transform, dataStore) => ContentProcessor.Process(ds.GetContent(transform), new ScopedDataStore(dataStore, Path.GetDirectoryName(transform))));
            var tmpDataStore = new ScopedDataStore(scopedTestCaseDataStore, ".tmp");

            var testCaseContent = testCaseDataStore.GetRequiredContent(".test");
            var testModel = JsonConvert.DeserializeObject<TestCaseModel>(testCaseContent);
            if (testModel.Actions == null)
            {
                var v1Model = JsonConvert.DeserializeObject<TestCaseModelV1>(testCaseContent);
                testModel = new(v1Model.Tags, v1Model.Parameters?.Actions);
            }

            Require.NotNull(testModel.Actions, $"No actions found for test {id}");
            var actions = testModel.Actions.Select(Parse).ToList();

            return new TestCase(id, testModel.Tags.ToEmptyIfNull(), actions, testCaseDataStore, tmpDataStore, _dataStoreProvider);
        }

        public IReadOnlyCollection<string> GetTestCaseIds()
        {
            _logger?.LogTrace("Getting TestCase ids");
            return DataStorage.GetAllGroups().Concat(new[] { "." }).Where(_ => _isTestCasePredicate(new ScopedDataStore(DataStorage, _))).ToList();
        }

        private record TestCaseModel(string[] Tags, string[] Actions);

        private class TestCaseModelV1
        {
            public string[] Tags { get; set; }

            public ParametersRecord Parameters { get; set; }

            public record ParametersRecord(string[] Actions);
        }

        private static ResolvableItemDescription Parse(string definition)
        {
            (var actionName, var queryString) = definition.SplitOnFirstOptional("?");
            var parametersAsNVC = HttpUtility.ParseQueryString(queryString);
            var parameters = parametersAsNVC.AllKeys.ToDictionary(key => key, key => (object)parametersAsNVC[key]);
            return new ResolvableItemDescription(actionName, parameters);
        }
    }
}
