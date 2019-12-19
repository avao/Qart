using System;
using System.Collections.Generic;
using System.Text;

namespace Qart.Core.Text
{
    public static class StringExtensions
    {
        public static string LeftOf(this string value, string token)
        {
            string result = value.LeftOfOptional(token);
            if (result == value)
            {
                throw new ArgumentException("String does not contain requested token [" + token + "]");
            }
            return result;
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
            var pos = value.IndexOf(token, StringComparison.InvariantCulture);
            if (pos == -1)
            {
                return value;
            }
            return value.Substring(pos + token.Length);
        }

        public static string LeftOfOptional(this string value, string token)
        {
            var pos = value.IndexOf(token, StringComparison.InvariantCulture);
            if (pos == -1)
            {
                return value;
            }
            return value.Substring(0, pos);
        }

        public static string Between(this string value, string leftToken, string rightToken)
        {
            var leftPos = value.IndexOf(leftToken, StringComparison.InvariantCulture);
            if (leftPos == -1)
            {
                throw new ArgumentException("String does not contain requested token [" + leftToken + "]");
            }

            var rightPos = value.IndexOf(rightToken, leftPos + leftToken.Length, StringComparison.InvariantCulture);
            if (rightPos == -1)
            {
                throw new ArgumentException("String does not contain requested token [" + rightToken + "]");
            }

            int startPos = leftPos + leftToken.Length;

            return value.Substring(startPos, rightPos - startPos);
        }

        public static (string, string) SplitOnLast(this string value, string token)
        {
            var pos = value.LastIndexOf(token, StringComparison.InvariantCulture);
            return value.SplitOnIndex(pos, token.Length);
        }

        public static (string, string) SplitOnFirst(this string value, string token)
        {
            var pos = value.IndexOf(token, StringComparison.InvariantCulture);
            return value.SplitOnIndex(pos, token.Length);
        }

        private static (string, string) SplitOnIndex(this string value, int index, int tokenLength)
        {
            return (value.Substring(0, index), value.Substring(index + tokenLength));
        }


        public static string SubstringWhile(this string value, Func<char, bool> predicate)
        {
            var builder = new StringBuilder();
            foreach (char c in value)
            {
                if (predicate(c))
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


        public static string SubstringUpTo(this string value, int startIndex, int maxLength)
        {
            return value.Substring(startIndex, Math.Min(maxLength, value.Length - startIndex));
        }

        public static bool IsXml(this string content)
        {
            return content.StartsWith("<?xml", StringComparison.InvariantCulture);
        }


        public static T ToEnum<T>(this string value)
            where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static string ToCsv(this IEnumerable<string> items)
        {
            return string.Join(",", items);
        }

        public static string ToMultiLine(this IEnumerable<string> items)
        {
            return string.Join(Environment.NewLine, items);
        }
    }
}
