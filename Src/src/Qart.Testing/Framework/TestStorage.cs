using Microsoft.Extensions.Logging;
using Qart.Core.DataStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qart.Testing.Framework
{
    public class TestStorage : ITestStorage
    {
        public IDataStore DataStorage { get; private set; }

        public IContentProcessor ContentProcessor { get; private set; }

        private readonly Func<IDataStore, bool> _isTestCasePredicate;
        private readonly IDataStoreProvider _dataStoreProvider;
        private readonly ILogger _logger;

        public TestStorage(IDataStore dataStorage, Func<IDataStore, bool> isTestCasePredicate, IContentProcessor processor, IDataStoreProvider dataStoreProvider, ILoggerFactory loggerFactory)
        {
            _isTestCasePredicate = isTestCasePredicate;
            ContentProcessor = processor;
            DataStorage = dataStorage;
            _dataStoreProvider = dataStoreProvider;

            _logger = loggerFactory.CreateLogger<TestStorage>();
        }

        public TestCase GetTestCase(string id)
        {
            _logger.LogTrace("Creating TestCase {id}", id);

            var scopedTestCaseDataStore = new ScopedDataStore(DataStorage, id);
            var testCaseDataStore = new ExtendedDataStore(scopedTestCaseDataStore, (ds, transform, dataStore) => ContentProcessor.Process(ds.GetContent(transform), new ScopedDataStore(dataStore, Path.GetDirectoryName(transform))));
            var tmpDataStore = new ScopedDataStore(scopedTestCaseDataStore, ".tmp");
            return new TestCase(id, this, testCaseDataStore, tmpDataStore, _dataStoreProvider);
        }

        public IEnumerable<string> GetTestCaseIds()
        {
            _logger.LogTrace("Getting TestCase ids");
            return DataStorage.GetAllGroups().Concat(new[] { "." }).Where(_ => _isTestCasePredicate(new ScopedDataStore(DataStorage, _)));
        }
    }
}
