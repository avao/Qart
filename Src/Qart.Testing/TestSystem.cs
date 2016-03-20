using Qart.Core.DataStore;
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
            var testCases = GetTestCases("");
            if (!testCases.Any())
            {
                var testCase = GetTestCase(".");
                if (testCase.Contains(".test"))
                {
                    testCases = new[] { testCase };
                }
            }
            return testCases;
        }

        private IEnumerable<TestCase> GetTestCases(string groupId)
        {
            var testCases = new List<TestCase>();
            var groups = DataStorage.GetItemGroups(groupId);
            foreach (var group in groups)
            {
                var id = Path.Combine(groupId, group);
                testCases.AddRange(GetTestCases(id));
                if(IsTestCase(id))
                {
                    testCases.Add(new TestCase(id, this));
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
