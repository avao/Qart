using Qart.Core.DataStore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qart.Testing
{
    public class TestSystem
    {
        public IDataStore DataStorage { get; private set; }

        public TestSystem(IDataStore dataStorage)
        {
            DataStorage = new ExtendedDataStore(dataStorage, (content, dataStore) => null);
        }

        public TestSystem(IDataStore dataStorage, IContentProcessor processor)
        {
            DataStorage = new ExtendedDataStore(dataStorage, (content, dataStore) => processor.Process(content, dataStore));
        }

        public TestCase GetTestCase(string id)
        {
            return new TestCase(id, this);
        }

        public IEnumerable<TestCase> GetTestCases()
        {
            return DataStorage.GetAllGroups().Concat(new[]{"."}).Where(_ => DataStorage.Contains(Path.Combine(_, ".test"))).Select(_ => new TestCase(_, this));
        }
    }
}
