using NUnit.Framework;
using Qart.Core.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Tests.Io
{
    [TestFixture]
    public class FileBasedStoreTests
    {
        [Test]
        public void NonExistingFileDoesNotThrow()
        {
            var baseDir = PathUtils.ResolveRelative(@"TestData\FileBasedStore");
            var store = new FileBasedStore(Path.Combine(baseDir, "aFile.txt"));
            store.ReplaceContent("a string " + DateTime.UtcNow);
        }

        [Test]
        public void ExistingFile()
        {
            var baseDir = PathUtils.ResolveRelative(@"TestData\FileBasedStore");
            var store = new FileBasedStore(Path.Combine(baseDir, "aFile.txt"));
        }
    }
}
