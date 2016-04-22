using Qart.Core.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io
{
    public static class Search
    {
        public static  IEnumerable<string> FindFiles(string pattern)
        {
            //TODO
            //Path.IsPathRooted(pattern);

            var tokens = PatternUtils.Tokenize(pattern);
            if(tokens.Length <2)
                return Enumerable.Empty<string>();
            string ds = Path.DirectorySeparatorChar.ToString();
            var first = tokens[0];
            if(first.EndsWith (":", StringComparison.Ordinal))
            {
                // if the token is a windows drive, add a backslash to make it an absolute path.
                first += ds;
            }
            else if(pattern.StartsWith(ds, StringComparison.Ordinal))
            {
                // if the token was originally an absolute path (unix) or absolute path
                // minus the drive letter (windows) add the slash/backslash again.
                first = ds + first;
            }

            return FindFiles( first, tokens.Skip(1)).ToList();
        }

        private static IEnumerable<string> FindFiles(string path, IEnumerable<string> tokens)
        {
            var first = tokens.First();
            var nextpath = Path.Combine(path, first);
            var rest = tokens.Skip(1).ToList();
            if (rest.Count > 0)
            {
                return Directory.EnumerateDirectories(path, first).SelectMany(_ => FindFiles(_, rest));
            }
            else
            {
                return Directory.EnumerateFiles(path, first);
            }
        }
    }
}
