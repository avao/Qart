using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static FileStream OpenFileStreamForWritingNoTruncate(string path)
        {
            FileUtils.EnsureCanBeWritten(path);
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public static FileStream OpenFileStreamForWriting(string path)
        {
            FileUtils.EnsureCanBeWritten(path);
            return new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        public static int ReadFromFile(string path, int length, byte[] buf)
        {
            using (var stream = OpenFileStreamForReading(path))
            {
                return stream.Read(buf, 0, length);
            }
        }

        public static void WriteAllText(string path, string content)
        {
            FileUtils.EnsureCanBeWritten(path);
            File.WriteAllText(path, content);
        }

        public static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
