using NUnit.Framework;
using Qart.Core.Io;
using System;
using System.IO;

namespace Qart.Core.Tests.Io
{
    public class FileBasedStoreTests
    {
        static readonly string BaseDir = PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "FileBasedStore"));

        [Test]
        public void NonExistingFileDoesNotThrow()
        {
            var store = new FileBasedStore(Path.Combine(BaseDir, "aFile.txt"));
            store.ReplaceContent("a string " + DateTime.UtcNow);
        }

        [Test]
        public void ExistingFile()
        {
            var store = new FileBasedStore(Path.Combine(BaseDir, "aFile.txt"));
        }
    }
}
