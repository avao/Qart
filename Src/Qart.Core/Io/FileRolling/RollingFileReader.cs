using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
    public class FileId
    {
        public string BaseFileName { get; private set; }
        public DateTime WriteTime { get; private set; }

        public FileId(string baseFileName, DateTime writeTime)
        {
            //TODO check that is not rolled file, i.e. "blah.5"
            BaseFileName = baseFileName;
            WriteTime = writeTime;
        }

        public static FileId Create(string baseFileName)
        {
            return new FileId(baseFileName, DateTime.UtcNow);
        }
    }

    public enum ReadBehaviour
    {
        FromBeginning,
        FromWhereLeft,
        FromCurrentPos
    }

    public class FilePosition
    {
        public long Position { get; private set; }
        public FileId FileId { get; private set; }

        public FilePosition(FileId fileId, long position)
        {
            Position = position;
            FileId = fileId;
        }

        public void UpdatePosition(long pos)
        {
            Position = pos;
        }
    }

    public class FilePositionSerialiser
    {
        public static FilePosition Read(string baseFileName, StreamReader reader)
        {
            var writeTime = reader.ReadDateTime();
            reader.Read();//skip space
            var pos = long.Parse(reader.ReadToEnd());
            return new FilePosition(new FileId(baseFileName, writeTime), pos);
        }

        public static void Write(FilePosition pos, TextWriter writer)
        {
            writer.Write(pos.FileId.WriteTime.ToString("MM/dd/yyyyTHH:mm:ss.fff"));
            writer.Write(" ");
            writer.Write(pos.Position);
        }

    }

    public static class TextReaderExtensions
    {
        public static DateTime ReadDateTime(this TextReader reader)
        {
            char[] buf = new char[23];
            reader.Read(buf, 0, 23);
            return DateTime.ParseExact(new string(buf), "dd/MM/yyyyTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }

    public interface IFilePositionStore
    {
        FilePosition GetPosition(string baseFileName);
        void SetPosition(FileId fileId, long pos);
    }


    public class FileBasedPositionStore : IFilePositionStore
    {
        //TODO concurrency
        private IDictionary<string, FilePosition> _positions;

        public string BaseDir { get; private set; }

        public FileBasedPositionStore(string baseDir)
        {
            BaseDir = baseDir;
            _positions = new Dictionary<string, FilePosition>();
        }

        public FilePosition GetPosition(string baseFileName)
        {
            FilePosition item;
            if (_positions.TryGetValue(baseFileName, out item))
            {
                return item;
            }
            else
            {
                string fileName = GetTargetFileName(baseFileName);
                if (File.Exists(fileName))
                {
                    using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                    using (var reader = new StreamReader(stream, Encoding.UTF8, false, 200, true))
                    {
                        return FilePositionSerialiser.Read(baseFileName, reader);
                    }
                }

                return null;
            }
        }

        public void SetPosition(FileId fileId, long pos)
        {
            var position = new FilePosition(fileId, pos);
            _positions[fileId.BaseFileName] = position;

            using (var stream = new FileStream(GetTargetFileName(fileId.BaseFileName), FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                FilePositionSerialiser.Write(position, writer);
            }
        }

        private string GetTargetFileName(string fileName)
        {
            //TODO include dir as well

            return Path.Combine(BaseDir, "_" + Path.GetFileName(fileName));
        }
    }

    public class RollingFileReader
    {
        IFilePositionStore _store;
        private string _baseFileName;
        private FilePosition _position;
        private FileStream _fileStream;
        private FileStream FileStream
        {
            get
            {
                return _fileStream;
            }
            set
            {
                if(_fileStream!=null)
                {
                    _fileStream.Close();//TODO
                    _fileStream.Dispose();
                }
                _fileStream = value;
            }
        }

        private class SingleFileInfo
        {
            public FileStream FileStream { get; private set; }
            public FileId Id { get; private set; }

            public SingleFileInfo(FileStream fileStream, FileId id)
            {
                FileStream = fileStream;
                Id = id;
            }
        }


        public RollingFileReader(string baseFileName, IFilePositionStore store, ReadBehaviour readBehaviour)
        {
            //TODO UNCs are not allowed

            _store = store;
            _baseFileName = baseFileName;
            switch (readBehaviour)
            {
                case ReadBehaviour.FromBeginning:
                    _position = new FilePosition(new FileId(baseFileName, DateTime.MinValue), 0);
                    break;
                case ReadBehaviour.FromWhereLeft:
                    _position = _store.GetPosition(baseFileName);
                    if (_position == null)
                    {
                        _position = new FilePosition(new FileId(baseFileName, DateTime.MinValue), 0);
                    }
                    break;
                case ReadBehaviour.FromCurrentPos:
                    var fileInfo = new FileInfo(baseFileName); //TODO what if not there??
                    var fileId = new FileId(baseFileName, fileInfo.LastWriteTimeUtc);
                    long pos = fileInfo.Length;
                    _position = new FilePosition(fileId, pos);
                    break;
                default:
                    throw new NotSupportedException("Unsupported ReadBehaviour");
            }

            FileStream = OpenFileStream(_position);
        }

        private static IEnumerable<string> GetFileNamesForBaseName(string baseFileName)
        {
            string fullPath = PathUtils.ResolveRelative(baseFileName);
            return Directory.GetFiles(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath) + ".*", SearchOption.TopDirectoryOnly);
        }

        private static IEnumerable<string> OrderByWriteTime(IEnumerable<string> fileNames)
        {
            //TODO
            //return fileNames.OrderBy(_ => File.GetLastWriteTimeUtc(_));
            return fileNames.OrderByDescending(_ => _);
        }

        
        private static string GetFullFileName(FileId fileId)
        {
            var fileNames = OrderByWriteTime(GetFileNamesForBaseName(fileId.BaseFileName));
            foreach (var fileName in fileNames)
            {
                var lastWriteTime = File.GetLastWriteTimeUtc(fileName);
                if (lastWriteTime >= fileId.WriteTime)
                {
                    return fileName;
                }
            }
            return null;
        }

        private static FileStream OpenFileStream(FileId fileId)
        {
            string fileName = GetFullFileName(fileId);
            if (fileName == null)
                return null;

            var stream = FileUtils.OpenFileStreamForReading(fileName);

            string fileName2 = GetFullFileName(fileId);
            if (fileName != fileName2)
            {//file has rolled
                //TODO do it 
                stream = FileUtils.OpenFileStreamForReading(fileName2);
            }
            return stream;
        }

        private static FileStream OpenFileStream(FilePosition position)
        {
            var stream = OpenFileStream(position.FileId);
            if(stream != null)
                stream.Position = position.Position;
            return stream;
        }


        public int Read(byte[] buf, int offset, int count)
        {
            if(_fileStream == null)
            {
                FileStream = OpenFileStream(_position);
            }

            if (_fileStream != null)
            {
                var result = _fileStream.Read(buf, offset, count);
                if (result != 0)
                    return result;

                var info1 = GetNexFileWriteTime(_position.FileId);
                if (info1 != null)
                {
                    var fileId = new FileId(_position.FileId.BaseFileName, info1.Item2);
                    FileStream fileStream = OpenFileStream(fileId);
                    var info2 = GetNexFileWriteTime(_position.FileId);
                    if (info1.Item1 != info2.Item1)
                    {
                        //rolled
                        fileId = new FileId(_position.FileId.BaseFileName, info2.Item2);
                        fileStream = OpenFileStream(fileId);
                    }
                    _position = new FilePosition(fileId, 0);
                    FileStream = fileStream;
                    return _fileStream.Read(buf, offset, count);
                }
            }
            return 0;
        }

        private static Tuple<string, DateTime> GetNexFileWriteTime(FileId fileId)
        {
            var fileNames = GetFileNamesForBaseName(fileId.BaseFileName).OrderByDescending(_ => _).ToList();
            for (int i = 0; i < fileNames.Count; ++i)
            {
                string fileName = fileNames[i];
                var lastWriteTime = File.GetLastWriteTimeUtc(fileName);
                if (lastWriteTime >= fileId.WriteTime)
                {
                    if (i == fileNames.Count - 1)
                        return null;
                    return new Tuple<string, DateTime>(fileNames[i + 1], lastWriteTime + TimeSpan.FromMilliseconds(1));
                }
            }
            return null;
        }

        public void Ack()
        {
            if (_fileStream != null && _position.Position != _fileStream.Position)
            {
                _position.UpdatePosition(_fileStream.Position);
                _store.SetPosition(_position.FileId, _fileStream.Position);
            }
        }
    }
}
