﻿using Qart.Core.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.FileBased
{
    public class DataStore : IDataStore
    {
        public string BasePath { get; private set; }

        public DataStore(string basePath)
        {
            BasePath = basePath;
        }

        public Stream GetReadStream(string itemId)
        {
            return FileUtils.OpenFileStreamForReading(GetAbsolutePath(itemId));
        }

        public Stream GetWriteStream(string itemId)
        {
            return FileUtils.OpenFileStreamForWriting(GetAbsolutePath(itemId));
        }

        public bool Contains(string itemId)
        {
            return File.Exists(GetAbsolutePath(itemId));
        }

        private string GetAbsolutePath(string itemId)
        {
            return Path.Combine(BasePath, itemId);
        }

        public IEnumerable<string> GetItemIds(string tag)
        {
            return Directory.EnumerateFiles(GetAbsolutePath(tag)).Select(Path.GetFileName).ToList();
        }

        public IEnumerable<string> Tags
        {
            get { return Directory.EnumerateDirectories(BasePath).Select(Path.GetFileName).ToList(); }
        }
    }
}