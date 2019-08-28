using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using System.IO;
using System.Linq;
using Qart.Testing.Framework;

namespace Qart.Testing.Tests
{
    class TestSystemTests
    {
        private readonly ITestStorage TestSystem = new TestStorage(new FileBasedDataStore(PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "TestCases"))), _ => _.Contains(".test"), null, null);

        [Test]
        public void GetTestCases()
        {
            var testCases = TestSystem.GetTestCaseIds();
            Assert.That(testCases.Count(), Is.GreaterThanOrEqualTo(1));
            Assert.That(testCases.FirstOrDefault(_ => _ == "ATestCase"), Is.Not.Null);
        }
    }
}
