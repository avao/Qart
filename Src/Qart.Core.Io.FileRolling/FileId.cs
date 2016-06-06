using System;
using System.Collections.Generic;
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
}
