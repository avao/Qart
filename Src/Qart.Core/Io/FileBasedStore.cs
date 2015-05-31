using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qart.Core.Text;

namespace Qart.Core.Io
{
    public class FileBasedStore
    {
        private const string BakFileToken = "._bak_.";
        private const string NewFileToken = "._new_.";

        public string FileName { get; private set; }

        public FileBasedStore(string fileName)
        {
            FileName = fileName;

            var dir = Path.GetDirectoryName(FileName);
            var name = Path.GetFileName(FileName);

            var newestFile = Directory.EnumerateFiles(dir, name + NewFileToken + "*").OrderByDescending(_ => File.GetLastWriteTime(_)).FirstOrDefault();
            if (newestFile != null)
            {
                //TODO reading file writetime twice
                if(File.GetLastWriteTime(fileName) < File.GetLastWriteTime(newestFile))
                {//try to recover
                    var postfix = newestFile.SubstringAfter(NewFileToken);
                    var bakFileName = FileName + BakFileToken + postfix;
                    RotateFiles(bakFileName, newestFile);
                }
                else
                {
                    //outdated newestFile
                }
            }

            foreach(var file in Directory.EnumerateFiles(dir, name + BakFileToken + "*"))
            {
                try
                {
                    File.Delete(file);
                }
                catch(Exception)
                {
                    //TODO log
                }
            }
            
        }

        public void ReplaceContent(string content)
        {
            var postfix = DateTime.UtcNow.Ticks.ToString();
            var bakFileName = FileName + BakFileToken + postfix;
            var newFileName = FileName + NewFileToken + postfix;

            File.WriteAllText(newFileName, content);

            RotateFiles(bakFileName, newFileName);
        }

        private void RotateFiles(string bakFileName, string newFileName)
        {
            if (File.Exists(FileName))
            {
                File.Move(FileName, bakFileName);
            }

            File.Move(newFileName, FileName);
            File.Delete(bakFileName);
        }

    }
}
