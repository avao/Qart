using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

            var testModel = JsonConvert.DeserializeObject<TestCaseModel>(testCaseDataStore.GetRequiredContent(".test"));
            var tags = testModel.Tags ?? Array.Empty<string>();

            Require.NotNull(testModel.Actions, $"No actions found for test {id}");
            var actions = testModel.Actions.Select(Parse).ToList();

            return new TestCase(id, tags, actions, testCaseDataStore, tmpDataStore, _dataStoreProvider);
        }

        public IReadOnlyCollection<string> GetTestCaseIds()
        {
            _logger?.LogTrace("Getting TestCase ids");
            return DataStorage.GetAllGroups().Concat(new[] { "." }).Where(_ => _isTestCasePredicate(new ScopedDataStore(DataStorage, _))).ToList();
        }

        private class TestCaseModel
        {
            public string[] Tags { get; set; }
            public string[] Actions { get; set; }
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
