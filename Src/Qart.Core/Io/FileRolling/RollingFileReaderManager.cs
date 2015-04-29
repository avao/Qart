using Qart.Core.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io.FileRolling
{
    public interface IOutputProvider
    {
        Stream GetOutput(string path);
    }

    public class DummyOutputProvider:IOutputProvider
    {
        public string BaseDir { get; private set; }
        public DummyOutputProvider(string baseDir)
        {
            BaseDir = baseDir;
        }

        public Stream GetOutput(string path)
        {
            var outputPath = Path.Combine(BaseDir, path.Replace('\\', '_').Replace(':', '_'));
            FileUtils.EnsureCanBeWritten(outputPath);
            return new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        }
    }


    public class RollingFileReaderManager
    {
        private FileWatcher _fileWatcher;
        private IDictionary<string, TimeBasedPoller> _knownFiles;
        private IFilePositionStore _positionStore;
        private ReadBehaviour _readBehaviour;
        private IOutputProvider _outputProvider;

        public RollingFileReaderManager(string pattern, IFilePositionStore store, ReadBehaviour readBehaviour, IOutputProvider outputProvider)
        {
            _positionStore = store;
            _knownFiles = new Dictionary<string, TimeBasedPoller>();
            _readBehaviour = readBehaviour;
            _outputProvider = outputProvider;
            _fileWatcher = new FileWatcher(pattern, OnNewFile);
            foreach (var file in Search.FindFiles(pattern))
            {
                OnNewFile(file);
            }
        }

       

        private void OnNewFile(string path)
        {
            TimeBasedPoller reader;
            if(IsRolledFile(path) || _knownFiles.TryGetValue(path, out reader) )
                return;

            var output = _outputProvider.GetOutput(path);
            _knownFiles.Add(path, new TimeBasedPoller(new RollingFileReader(path, _positionStore, _readBehaviour), (buf, pos, len) => { output.Write(buf, pos, len); output.Flush(); return true;}));
        }

        private bool IsRolledFile(string path)
        {
            return false; //TODO
        }
    }



    public class RollingFileTextReaderManager
    {
        private FileWatcher _fileWatcher;
        private IDictionary<string, TimeBasedPollerForLineReader> _knownFiles;
        private IFilePositionStore _positionStore;
        private ReadBehaviour _readBehaviour;
        private Func<string, string, bool> _func;

        public RollingFileTextReaderManager(string pattern, IFilePositionStore store, ReadBehaviour readBehaviour, Func<string, string, bool> func)
        {
            _positionStore = store;
            _knownFiles = new Dictionary<string, TimeBasedPollerForLineReader>();
            _readBehaviour = readBehaviour;
            _func = func;
            _fileWatcher = new FileWatcher(pattern, OnNewFile);
            foreach (var file in Search.FindFiles(pattern))
            {
                OnNewFile(file);
            }
        }

        private void OnNewFile(string path)
        {
            TimeBasedPollerForLineReader reader;
            if (IsRolledFile(path) || _knownFiles.TryGetValue(path, out reader))
                return;

            _knownFiles.Add(path, new TimeBasedPollerForLineReader(new RollingFileReader(path, _positionStore, _readBehaviour), (line) => { return _func(path,line); }));
        }

        private bool IsRolledFile(string path)
        {
            return false; //TODO
        }
    }
}
