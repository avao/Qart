using Moq;
using NUnit.Framework;
using Qart.Core.Io;
using Qart.Core.Io.FileRolling;
using Qart.Testing;
using System.IO;

namespace Qart.Core.Tests.Io.FileRolling
{
    [TestFixture]
    public class RollingFileReaderTests
    {
        readonly string BaseDir = PathUtils.ResolveRelativeToAssmeblyLocation(Path.Combine("TestData", "RollingFileReader"));

        [Test, Ignore("TODO")]
        public void RolledFilesNoPosition()
        {
            string testDir = Path.Combine(BaseDir, "RolledFilesWithoutPosition");
            var path = Path.Combine(testDir, "aFile.txt");

            var filePositionStoreMock = new Mock<IFilePositionStore>();
            var fileReader = new RollingFileReader(path, filePositionStoreMock.Object, ReadBehaviour.FromBeginning);
            string text = fileReader.ReadAllText();

            BaseLineAssert.AreEqual(Path.Combine(testDir, "expected.txt"), text);
        }

        [Test, Ignore("TODO")]
        public void RolledFilesWithPosition()
        {
            string testDir = Path.Combine(BaseDir, "RolledFilesWithPosition");

            var path = Path.Combine(testDir, "aFile.txt");

            FilePosition position;
            using (var stream = FileUtils.OpenFileStreamForReading(Path.Combine(testDir, "_aFile.txt")))
            using (var reader = new StreamReader(stream))
            {
                position = FilePositionSerialiser.Read(path, reader);
            }
            var filePositionStoreMock = new Mock<IFilePositionStore>();
            filePositionStoreMock.Setup(_ => _.GetPosition(path)).Returns(position);
            var fileReader = new RollingFileReader(path, filePositionStoreMock.Object, ReadBehaviour.FromWhereLeft);
            string text = fileReader.ReadAllText();

            BaseLineAssert.AreEqual(Path.Combine(testDir, "expected.txt"), text);
        }
    }

    public static class RollingFileReaderExtensions
    {
        public static string ReadAllText(this RollingFileReader reader)
        {
            using (var stream = new MemoryStream())
            {
                reader.CopyAllTextTo(stream);
                stream.Position = 0;
                using (var reader2 = new StreamReader(stream))
                {
                    return reader2.ReadToEnd();
                }
            }
        }

        public static void CopyAllTextTo(this RollingFileReader reader, MemoryStream writer)
        {
            const int bufSize = 1000;
            var buf = new byte[bufSize];

            var len = reader.Read(buf, 0, bufSize);
            writer.Write(buf, 0, len);
            reader.Ack();
            while (len > 0)
            {
                len = reader.Read(buf, 0, bufSize);
                writer.Write(buf, 0, len);
                reader.Ack();
            }
        }
    }
}
