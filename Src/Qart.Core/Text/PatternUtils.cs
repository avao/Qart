using System;
using System.Linq;

namespace Qart.Core.Text
{
    public static class PatternUtils
    {
        public static bool IsPattern(string token)
        {
            return token.Contains('*') || token.Contains('?');
        }

        public static string[] Tokenize(string pattern)
        {
            return pattern.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
