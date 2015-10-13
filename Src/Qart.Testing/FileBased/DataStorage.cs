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

        public Stream GetStream(string itemId)
        {
            return FileUtils.OpenFileStreamForReading(GetAbsolutePath(itemId));
        }

        public void PutContent(string itemId, string content)
        {
            FileUtils.WriteAllText(GetAbsolutePath(itemId), content);
        }


        private string GetAbsolutePath(string itemId)
        {
            return Path.Combine(BasePath, itemId);
        }

    }
}
