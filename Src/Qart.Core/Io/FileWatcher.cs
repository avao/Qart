using Qart.Core.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Qart.Core.Io
{
    public class FileWatcher
    {
        
        private Queue<KeyValuePair<string, IEnumerable<string>>> _queue;

        private FileSystemWatcher _fileWatcher;
        private string _pattern;
        private Regex _regex;
        private Action<string> _action;


        public FileWatcher(string pattern, Action<string> action)
        {
            _action = action;

            //TODO check if rooted
            _regex = new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                            RegexOptions.IgnoreCase | RegexOptions.Singleline);
            _pattern = pattern;
            _queue = new Queue<KeyValuePair<string, IEnumerable<string>>>();
            var tokens = PatternUtils.Tokenize(pattern);
            _queue.Enqueue(new KeyValuePair<string, IEnumerable<string>>(tokens[0] +  Path.DirectorySeparatorChar, tokens.Skip(1)));

            CreateWatchers();
        }

        private void CreateWatchers()
        {
            while(_queue.Count>0)
            {
                var kvp = _queue.Dequeue();
                var first = kvp.Value.First();
                var path = Path.Combine(kvp.Key, first);
                if(!PatternUtils.IsPattern(first) && Directory.Exists(path))
                {
                    var rest = kvp.Value.Skip(1).ToList();
                    if(rest.Count>0)
                    {
                        _queue.Enqueue(new KeyValuePair<string,IEnumerable<string>>(path, rest));
                    }
                    else
                    {
                        _fileWatcher = CreateWatcher(path, rest.Count == 1 ? rest[0] : "");
                    }
                }
                else
                {
                    _fileWatcher = CreateWatcher(kvp.Key, "*");
                }
            }
        }

        private FileSystemWatcher CreateWatcher(string path, string filter)
        {
            var fileWatcher = new FileSystemWatcher(path, filter);
            fileWatcher.NotifyFilter = NotifyFilters.FileName;// | NotifyFilters.LastWrite;
            fileWatcher.Created += OnFileCreate;
            fileWatcher.IncludeSubdirectories = true; //TODO optimize not to listen to all folders

            fileWatcher.EnableRaisingEvents = true;

            return fileWatcher;
        }

        

        private static string GetDirectoryForListening(string pattern)
        {
            var dirName = Path.GetDirectoryName(pattern);

            var asterixPos = pattern.IndexOf('*');
            if (asterixPos == -1)
                return dirName;

            string prefix = dirName.Substring(0, asterixPos);
            return Path.GetDirectoryName(prefix);
        }

        private void OnFileCreate(object source, FileSystemEventArgs e)
        {
            if (IsMatching(e.FullPath, _pattern))
            {
                _action(e.FullPath);
            }
        }

        private bool IsMatching(string value, string pattern)
        {
            return _regex.IsMatch(value);
        }

    }
}
