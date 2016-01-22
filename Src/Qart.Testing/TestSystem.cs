using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing
{
    public class TestSystem
    {
        public IDataStore DataStorage { get; private set; }

        public TestSystem(IDataStore dataStorage)
        {
            DataStorage = dataStorage;
        }

        public TestCase GetTestCase(string id)
        {
            return new TestCase(id, this);
        }

        public IEnumerable<TestCase> GetTestCases()
        {
            return GetTestCases("");
        }

        private IEnumerable<TestCase> GetTestCases(string groupId)
        {
            var testCases = new List<TestCase>();
            var groups = DataStorage.GetItemGroups(groupId);
            foreach (var group in groups)
            {
                testCases.AddRange(GetTestCases(group));
                if(IsTestCase(group))
                {
                    testCases.Add(new TestCase(group, this));
                }
            }
            return testCases;
        }

        private bool IsTestCase(string group)
        {
            return DataStorage.Contains(Path.Combine(group, ".test"));
        }
    }
}
