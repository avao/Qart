using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using System.IO;
using System.Text;

namespace Qart.Testing.Tests
{
    public class TestCaseTests
    {
        class TextStreamTranformer : IStreamTransformer
        {
            public Stream Transform(Stream strm)
            {
                return new MemoryStream(UTF8Encoding.UTF8.GetBytes("blah"));
            }
        }

        class StreamTransformResolver : IStreamTransformerResolver
        {
            public IStreamTransformer ResolveTransformer(string name, object param)
            {
                return new TextStreamTranformer();
            }
        }

        TestSystem TestSystem = new TestSystem(new FileBasedDataStore(PathUtils.ResolveRelative(Path.Combine("TestData", "TestCases"))), new ContentProcessor(new StreamTransformResolver()));

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

        [Test]
        public void Transform()
        {
            var testCase = TestSystem.GetTestCase("Transform");
            Assert.AreEqual("blah", testCase.GetContent("artifact.xml"));
        }
    }
}
