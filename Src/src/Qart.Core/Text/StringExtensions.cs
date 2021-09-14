using Qart.Core.Validation;
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
            RequireDifferent(result, value, token);
            return result;
        }

        public static string LeftOfLast(this string value, string token)
        {
            string result = value.LeftOfLastOptional(token);
            RequireDifferent(result, value, token);
            return result;
        }

        public static string RightOf(this string value, string token)
        {
            string result = value.RightOfOptional(token);
            RequireDifferent(result, value, token);
            return result;
        }
        public static string RightOfLast(this string value, string token)
        {
            string result = value.RightOfLastOptional(token);
            RequireDifferent(result, value, token);
            return result;
        }

        private static void RequireDifferent(string result, string value, string token)
        {
            Require.NotEqual(result, value, "String does not contain required token [" + token + "]");
        }

        public static string RightOfOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var pos = value.IndexOf(token, comparisonType);
            return RightOfOptionalIndex(value, pos, token);
        }

        public static string LeftOfOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            return LeftOfOptionalIndex(value, value.IndexOf(token, comparisonType));
        }

        public static string RightOfLastOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var index = value.LastIndexOf(token, comparisonType);
            return RightOfOptionalIndex(value, index, token);
        }

        public static string LeftOfLastOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            return LeftOfOptionalIndex(value, value.LastIndexOf(token, comparisonType));
        }

        public static string Between(this string value, string leftToken, string rightToken, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var leftPos = value.IndexOf(leftToken, comparisonType);
            RequireNonNegative(leftPos);

            var rightPos = value.IndexOf(rightToken, leftPos + leftToken.Length, comparisonType);
            RequireNonNegative(rightPos);

            int startPos = leftPos + leftToken.Length;

            return value.Substring(startPos, rightPos - startPos);
        }

        public static (string, string) SplitOnLast(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var index = value.LastIndexOf(token, comparisonType);
            RequireNonNegative(index);
            return value.SplitOnTokenIndex(index, token.Length);
        }

        public static (string, string) SplitOnFirst(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var index = value.IndexOf(token, comparisonType);
            RequireNonNegative(index);
            return value.SplitOnTokenIndex(index, token.Length);
        }

        private static void RequireNonNegative(int index)
        {
            Require.That(() => index != -1, "String does not contain requested token");
        }

        private static string RightOfOptionalIndex(this string value, int index, string token)
        {
            return index == -1 ? value : value.Substring(index + token.Length);
        }

        private static string LeftOfOptionalIndex(this string value, int index)
        {
            return index == -1 ? value : value.Substring(0, index);
        }

        private static (string, string) SplitOnTokenIndex(this string value, int index, int tokenLength)
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
