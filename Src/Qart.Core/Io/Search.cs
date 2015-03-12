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
            var first = tokens[0];
            if(first.EndsWith(":"))
            {
                first += @"\";
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
