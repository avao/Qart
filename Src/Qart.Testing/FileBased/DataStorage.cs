using Qart.Core.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.FileBased
{
    public class DataStorage : IDataStorage
    {
        public string BasePath { get; private set; }

        public DataStorage(string basePath)
        {
            BasePath = basePath;
        }

        public Stream GetStream(string testCaseId, string itemId)
        {
            return File.OpenRead(GetAbsolutePath(testCaseId, itemId));
        }

        public string GetContent(string testCaseId, string itemId)
        {
            string absolutePath = GetAbsolutePath(testCaseId, itemId);
            if (File.Exists(absolutePath))
            {
                return File.ReadAllText(absolutePath);
            }
            return string.Empty;
        }

        public void PutContent(string testCaseId, string itemId, string content)
        {
            FileUtils.WriteAllText(GetAbsolutePath(testCaseId, itemId), content);
        }


        private string GetAbsolutePath(string testCaseId, string itemId)
        {
            return Path.Combine(BasePath, testCaseId, itemId);
        }

    }
}
