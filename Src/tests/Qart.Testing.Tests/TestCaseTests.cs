using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Qart.Core.DataStore;
using Qart.Core.Io;
using Qart.Testing.Framework;
using System.IO;
using System.Text;

namespace Qart.Testing.Tests
{
    public class TestCaseTests
    {
        class TextStreamTranformer : IStreamTransformer
        {
            public Stream Transform(Stream strm, IDataStore dataStore, object param)
            {
                return new MemoryStream(Encoding.UTF8.GetBytes((string)param));
            }
        }

        class StreamTransformResolver : IStreamTransformerResolver
        {
            public IStreamTransformer GetTransformer(string name)
            {
                return new TextStreamTranformer();
            }

            public void Release(IStreamTransformer streamTransformer)
            {
            }
        }

        ITestStorage TestSystem = new TestStorage(new FileBasedDataStore(PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "TestCases"))), _ => true, new ContentProcessor(new StreamTransformResolver()), null, new LoggerFactory());

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
            Assert.AreEqual("hhh", testCase.GetContent("artifact.xml"));
        }
    }
}
