using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using System.IO;
using System.Linq;

namespace Qart.Core.Tests.DataStore
{
    public class ScopedDataStoreTests
    {
        [Test]
        public void ScopedDataStore()
        {
            var dataStore = new FileBasedDataStore(PathUtils.ResolveRelativeToAssmeblyLocation(@"TestData"));
            var scopedDataStore = new ScopedDataStore(dataStore, "TestCases");
            Assert.That(scopedDataStore.GetItemIds("Ref").Contains(Path.Combine("Ref", "artifact.xml.ref")), Is.True);
        }
    }
}
