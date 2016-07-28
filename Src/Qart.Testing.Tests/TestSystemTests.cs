using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Tests
{
    class TestSystemTests
    {
        readonly TestSystem TestSystem = new TestSystem(new FileBasedDataStore(PathUtils.ResolveRelative(Path.Combine("TestData", "TestCases"))));

        [Test]
        public void GetTestCases()
        {
            var testCases = TestSystem.GetTestCases();
            Assert.That(testCases.Count(), Is.GreaterThanOrEqualTo(1));
            Assert.That(testCases.FirstOrDefault(_ => _.Id == "ATestCase"), Is.Not.Null);
        }
    }
}
