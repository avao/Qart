using NUnit.Framework;
using Qart.Core.Io;
using Qart.Testing.FileBased;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Tests
{
    class DataStoreTests
    {
        [Test]
        public void ScopedDataStore()
        {
            var dataStore = new DataStore(PathUtils.ResolveRelative(@"TestData"));
            var scopedDataStore = new ScopedDataStore(dataStore, "TestCases");
            Assert.AreEqual(new[]{"Ref\\artifact.xml.ref"}, scopedDataStore.GetItemIds("Ref"));
        }

    }
}
