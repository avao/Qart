using NUnit.Framework;
using Qart.Core.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Tests.Io
{
    [TestFixture]
    public class SearchTests
    {
        private static string TestDataDir = PathUtils.ResolveRelative("TestData");

        [Test]
        static public void FindExact()
        {
            var pattern = TestDataDir + @"\FindFiles\ADirectory\NestedDirectory\AFile.txt";
            var files = Search.FindFiles(pattern).ToList();
            Assert.That(files.Count(), Is.EqualTo(1));
            Assert.That(files.First().EndsWith(pattern));
        }

        [Test]
        static public void FindMultiple()
        {
            var pattern = TestDataDir + @"\FindFiles\ADirectory\NestedDirectory\*";
            var files = Search.FindFiles(pattern).ToList();
            Assert.That(files.Count(), Is.EqualTo(2));
            Assert.That(files.First().EndsWith("AFile.txt"));
            Assert.That(files.Skip(1).First().EndsWith("AnotherFile.dat"));
        }
    }
}
