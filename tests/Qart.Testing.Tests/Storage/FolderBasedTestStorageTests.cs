using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using Qart.Testing.Storage;
using System.IO;
using System.Linq;

namespace Qart.Testing.Tests.Storage
{
    public class FolderBasedTestStorageTests
    {
        private readonly ITestStorage TestSystem = new DataStoreBasedTestStorage(new FileBasedDataStore(PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "TestCases"))), _ => _.Contains(".test"));

        [Test]
        public void GetTestCases()
        {
            var testCases = TestSystem.GetTestCaseIds();
            Assert.That(testCases.Count(), Is.GreaterThanOrEqualTo(1));
            Assert.That(testCases.FirstOrDefault(_ => _ == "Pipeline\\Diff"), Is.Not.Null);
        }
    }
}
