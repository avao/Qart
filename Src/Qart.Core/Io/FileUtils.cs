using System;
using System.IO;
using System.Reflection;

namespace Qart.Core.Io
{
    public static class FileUtils
    {
        public static void EnsureCanBeWritten(string path)
        {
            var dirName = Path.GetDirectoryName(path);
            if(!string.IsNullOrEmpty(dirName))
                Directory.CreateDirectory(dirName);
        }

        public static FileStream OpenFileStreamForReading(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }

        public static FileStream OpenFileStreamForWritingNoTruncate(string path)
        {
            return OpenFileStreamForWriting(path, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public static FileStream OpenFileStreamForWriting(string path)
        {
            return OpenFileStreamForWriting(path, FileMode.Create, FileAccess.Write);
        }

        private static FileStream OpenFileStreamForWriting(string path, FileMode fileMode, FileAccess fileAccess)
        {
            //if IO is slow (shared network folder) it is faster to try executing an action and then if directory does not exist catch exception and create one. 
            try
            {
                return new FileStream(path, fileMode, fileAccess);
            }
            catch (IOException ex)
            {
                //TODO more specific exception
            }
            
            FileUtils.EnsureCanBeWritten(path);
            return new FileStream(path, fileMode, fileAccess);
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
            EnsureCanBeWritten(path);
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
