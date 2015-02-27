using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Io
{
    public static class PathUtils
    {
        public static string ResolveRelative(string path)
        {
            if (Path.IsPathRooted(path))
                return path;

            string curDir = Directory.GetCurrentDirectory();
            while (curDir != null)
            {
                string fullPath = Path.Combine(curDir, path);
                if (File.Exists(fullPath) || Directory.Exists(fullPath))
                {
                    return fullPath;
                }
                curDir = Path.GetDirectoryName(curDir);
            }
            throw new IOException("Cannot resolve path.");
        }
    }
}
