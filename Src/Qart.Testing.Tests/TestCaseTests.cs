using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using System.IO;

namespace Qart.Testing.Tests
{
    public class TestCaseTests
    {
        TestSystem TestSystem = new TestSystem(new FileBasedDataStore(PathUtils.ResolveRelative(Path.Combine("TestData", "TestCases"))));

        [Test]
        public void Ref()
        {
            var testCase = TestSystem.GetTestCase("Ref");
            Assert.AreEqual("<root/>", testCase.GetContent("artifact.xml"));
        }

        [Test]
        public void MisssingArtefact()
        {
            var testCase = TestSystem.GetTestCase("Ref");
            Assert.IsNull(testCase.GetContent("missing"));
        }

    }
}
