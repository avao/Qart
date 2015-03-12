using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
