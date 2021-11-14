using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Qart.Core.Activation;
using Qart.Core.DataStore;
using Qart.Core.Io;
using Qart.Testing.Storage;
using Qart.Testing.Transformations;
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

        [Test]
        public void Ref()
        {
            var testCase = CreateTestStorage().GetTestCase("Ref");
            Assert.AreEqual("<root/>", testCase.GetContent("artifact.xml"));
        }

        [Test]
        public void MisssingArtefact()
        {
            var testCase = CreateTestStorage().GetTestCase("Ref");
            Assert.IsNull(testCase.GetContent("missing"));
        }

        [Test]
        public void Transform()
        {
            var testCase = CreateTestStorage().GetTestCase("Transform");
            Assert.AreEqual("hhh", testCase.GetContent("artifact.xml"));
        }

        ITestStorage CreateTestStorage()
        {
            var factoryMock = new Mock<IObjectFactory<IStreamTransformer>>();
            factoryMock.Setup(f => f.Create(It.IsAny<string>(), null)).Returns(new TextStreamTranformer());

            return new DataStoreBasedTestStorage(new FileBasedDataStore(PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "TestCases"))), _ => true, new ContentProcessor(factoryMock.Object));
        }
    }
}
