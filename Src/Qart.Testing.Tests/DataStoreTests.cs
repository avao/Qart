using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using System.IO;

namespace Qart.Testing.Tests
{
    class DataStoreTests
    {
        [Test]
        public void ScopedDataStore()
        {
            var dataStore = new FileBasedDataStore(PathUtils.ResolveRelative(@"TestData"));
            var scopedDataStore = new ScopedDataStore(dataStore, "TestCases");
            Assert.AreEqual(new[]{Path.Combine("Ref", "artifact.xml.ref")}, scopedDataStore.GetItemIds("Ref"));
        }

    }
}
