using Qart.Core.DataStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qart.Testing.Framework
{
    public class TestSystem : ITestSystem
    {
        public IDataStore DataStorage { get; private set; }

        public IContentProcessor ContentProcessor { get; private set; }

        private readonly Func<IDataStore, bool> _isTestCasePredicate;

        public TestSystem(IDataStore dataStorage)
            : this(dataStorage, _ => _.Contains(".test"))
        {
        }

        public TestSystem(IDataStore dataStorage, Func<IDataStore, bool> isTestCasePredicate)
            : this(dataStorage, isTestCasePredicate, null)
        {
        }

        public TestSystem(IDataStore dataStorage, Func<IDataStore, bool> isTestCasePredicate, IContentProcessor processor)
        {
            _isTestCasePredicate = isTestCasePredicate;
            ContentProcessor = processor;
            DataStorage = dataStorage;
        }

        public TestCase GetTestCase(string id)
        {
            var testCaseDataStore = new ExtendedDataStore(new ScopedDataStore(DataStorage, id), (content, dataStore) => ContentProcessor.Process(content, dataStore));
            return new TestCase(id, this, testCaseDataStore);
        }

        public IEnumerable<string> GetTestCaseIds()
        {
            return DataStorage.GetAllGroups().Concat(new[] { "." }).Where(_ => _isTestCasePredicate(new ScopedDataStore(DataStorage, _)));
        }
    }
}
