﻿using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;

namespace Qart.Core.Tests.DataStore
{
    public class ExtendedDataStoreTests
    {
        [Test]
        public void ResolvingAnItemSucceeds()
        {
            var baseDir = PathUtils.ResolveRelativeToAssmeblyLocation("TestData/TestCases/ExtendedDataStore");
            var store = new ExtendedDataStore(new FileBasedDataStore(baseDir));
            Assert.That(store.Contains("afile.xml"), Is.True);
            Assert.That(store.Contains("nonexistent"), Is.False);
            string content = store.GetContent("afile.xml");
            Assert.That(content, Is.EqualTo("<root/>"));
        }

    }
}
