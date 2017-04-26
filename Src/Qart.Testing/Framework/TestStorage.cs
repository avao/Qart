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

        public TestStorage(IDataStore dataStorage, Func<IDataStore, bool> isTestCasePredicate, IContentProcessor processor, IDataStoreProvider dataStoreProvider)
        {
            _isTestCasePredicate = isTestCasePredicate;
            ContentProcessor = processor;
            DataStorage = dataStorage;
            _dataStoreProvider = dataStoreProvider;
        }

        public TestCase GetTestCase(string id)
        {
            var testCaseDataStore = new ExtendedDataStore(new ScopedDataStore(DataStorage, id), (ds, transform, dataStore) => ContentProcessor.Process(ds.GetContent(transform), new ScopedDataStore(dataStore, Path.GetDirectoryName(transform))));
            return new TestCase(id, this, testCaseDataStore, _dataStoreProvider);
        }

        public IEnumerable<string> GetTestCaseIds()
        {
            return DataStorage.GetAllGroups().Concat(new[] { "." }).Where(_ => _isTestCasePredicate(new ScopedDataStore(DataStorage, _)));
        }
    }
}
