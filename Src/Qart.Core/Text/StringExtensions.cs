using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Text
{
    public static class StringExtensions
    {
        public static string SubstringAfter(this string value, string token)
        {
            var pos = value.IndexOf(token);
            if(pos == -1)
            {
                throw new ArgumentException("String does not contain requested token [" + token + "]");
            }
            return value.Substring(pos + token.Length);
        }
    }
}
