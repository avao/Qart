using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using System;
using System.IO;

namespace Qart.Core.Tests.DataStore
{
    [TestFixture]
    class ExtendedDataStoreTests
    {
        [Test]
        public void ResolvingAnItemSucceeds()
        {
            var baseDir = PathUtils.ResolveRelative("TestData/TestCases/ExtendedDataStore");
            var store = new ExtendedDataStore(new FileBasedDataStore(baseDir));
            Assert.That(store.Contains("afile.xml"), Is.True);
            Assert.That(store.Contains("nonexistent"), Is.False);
            string content = store.GetContent("afile.xml");
            Assert.That(content, Is.EqualTo("<root/>"));
        }

    }
}
