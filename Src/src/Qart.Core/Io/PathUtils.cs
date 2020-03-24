using System;
using System.IO;

namespace Qart.Core.Io
{
    public static class PathUtils
    {
        [Obsolete("Use one of the ResolveRelativeToCurrentDirectory/ResolveRelativeToAssmeblyLocation instead.")]
        public static string ResolveRelative(string path)
        {
            return ResolveRelativeToCurrentDirectory(path);
        }

        public static string ResolveRelativeToCurrentDirectory(string path)
        {
            return ResolveRelative(path, Directory.GetCurrentDirectory());
        }

        public static string ResolveRelativeToAssmeblyLocation(string path)
        {
            return ResolveRelative(path, FileUtils.GetAssemblyDirectory());
        }

        public static string ResolveRelative(string relativePath, string startingPath)
        {
            if (Path.IsPathRooted(relativePath))
                return relativePath;

            while (startingPath != null)
            {
                string fullPath = Path.Combine(startingPath, relativePath);
                if (File.Exists(fullPath) || Directory.Exists(fullPath))
                {
                    return fullPath;
                }
                startingPath = Path.GetDirectoryName(startingPath);
            }
            throw new IOException(string.Format("Cannot resolve path [{0}] with starting point [{1}]", relativePath, startingPath));
        }

        public static string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
