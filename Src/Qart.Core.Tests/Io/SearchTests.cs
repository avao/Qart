using NUnit.Framework;
using Qart.Core.Io;
using System.IO;
using System.Linq;

namespace Qart.Core.Tests.Io
{
    [TestFixture]
    public class SearchTests
    {
        static readonly string TestDataDir = PathUtils.ResolveRelativeToAssmeblyLocation("TestData");

        [Test]
        static public void FindExact()
        {
            var pattern = Path.Combine(TestDataDir, "FindFiles", "ADirectory", "NestedDirectory", "AFile.txt");
            var files = Search.FindFiles(pattern).ToList();
            Assert.That(files.Count(), Is.EqualTo(1));
            Assert.That(files.First().EndsWith(pattern));
        }

        [Test]
        static public void FindMultiple()
        {
            var pattern = Path.Combine(TestDataDir, "FindFiles", "ADirectory", "NestedDirectory", "*");
            var files = Search.FindFiles(pattern).ToList();
            Assert.That(files.Count(), Is.EqualTo(2));
            Assert.That(files.First().EndsWith("AFile.txt"));
            Assert.That(files.Skip(1).First().EndsWith("AnotherFile.dat"));
        }
    }
}
