using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Core.Text
{
    public static class StringExtensions
    {
        [Obsolete]
        public static string SubstringBefore(this string value, string token)
        {
            return value.LeftOf(token);
        }

        public static string LeftOf(this string value, string token)
        {
            string result = value.LeftOfOptional(token);
            if (result == value)
            {
                throw new ArgumentException("String does not contain requested token [" + token + "]");
            }
            return result;
        }


        [Obsolete]
        public static string SubstringAfter(this string value, string token)
        {
            return value.RightOf(token);
        }

        public static string RightOf(this string value, string token)
        {
            string result = value.RightOfOptional(token);
            if (result == value)
            {
                throw new ArgumentException("String does not contain requested token [" + token + "]");
            }
            return result;
        }

        public static string RightOfOptional(this string value, string token)
        {
            var pos = value.IndexOf(token);
            if (pos == -1)
            {
                return value;
            }
            return value.Substring(pos + token.Length);
        }

        public static string LeftOfOptional(this string value, string token)
        {
            var pos = value.IndexOf(token);
            if (pos == -1)
            {
                return value;
            }
            return value.Substring(0, pos);
        }

        public static string SubstringWhile(this string value, Func<char, bool> predicate)
        {
            var builder = new StringBuilder();
            foreach (char c in value)
            {
                if(predicate(c))
                {
                    builder.Append(c);
                }
                else
                {
                    break;
                }
            }
            return builder.ToString();
        }
    }
}
