using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io
{
    public static class FileUtils
    {
        public static void EnsureCanBeWritten(string path)
        {
            string dirName = Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dirName))
            {//Create it
                Directory.CreateDirectory(dirName);
            }
        }

        public static FileStream OpenFileStreamForReading(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }

        public static int ReadFromFile(string path, int length, byte[] buf)
        {
            using(var stream = OpenFileStreamForReading(path))
            {
                return stream.Read(buf, 0, length);
            }
        }
    }
}
