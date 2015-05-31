using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
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

            using (var stream = FileUtils.OpenFileStreamForWriting(GetTargetFileName(fileId.BaseFileName)))
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

        public void Dispose()
        {
            //TODO flush cached postions
        }
    }
}
