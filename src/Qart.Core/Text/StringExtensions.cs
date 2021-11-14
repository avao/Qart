using Qart.Core.Validation;
using System;
using System.Collections.Generic;

namespace Qart.Core.Text
{
    public static class StringExtensions
    {
        public static string LeftOf(this string value, string token)
        {
            return value.LeftOfOptional(token).RequireDifferent(value, token);
        }

        public static string LeftOfLast(this string value, string token)
        {
            return value.LeftOfLastOptional(token).RequireDifferent(value, token);
        }

        public static string RightOf(this string value, string token)
        {
            return value.RightOfOptional(token).RequireDifferent(value, token);
        }
        public static string RightOfLast(this string value, string token)
        {
            return value.RightOfLastOptional(token).RequireDifferent(value, token);
        }

        private static string RequireDifferent(this string result, string value, string token)
        {
            Require.NotEqual(result, value, "String does not contain required token [" + token + "]");
            return result;
        }

        public static string RightOfOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            return RightOfOptionalIndex(value, value.IndexOf(token, comparisonType), token);
        }

        public static string LeftOfOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            return LeftOfOptionalIndex(value, value.IndexOf(token, comparisonType));
        }

        public static string RightOfLastOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            return RightOfOptionalIndex(value, value.LastIndexOf(token, comparisonType), token);
        }

        public static string LeftOfLastOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            return LeftOfOptionalIndex(value, value.LastIndexOf(token, comparisonType));
        }

        public static string Between(this string value, string leftToken, string rightToken, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var leftPos = value.IndexOf(leftToken, comparisonType).RequireNonNegative();

            var rightPos = value.IndexOf(rightToken, leftPos + leftToken.Length, comparisonType).RequireNonNegative();

            int startPos = leftPos + leftToken.Length;

            return value.Substring(startPos, rightPos - startPos);
        }

        public static (string, string) SplitOnLast(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var index = value.LastIndexOf(token, comparisonType).RequireNonNegative();
            return value.SplitOnTokenIndex(index, token.Length);
        }

        public static (string, string) SplitOnFirst(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var index = value.IndexOf(token, comparisonType).RequireNonNegative();
            return value.SplitOnTokenIndex(index, token.Length);
        }

        public static (string, string) SplitOnFirstOptional(this string value, string token, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            var index = value.IndexOf(token, comparisonType);
            return index == -1 ? (value, string.Empty) : value.SplitOnTokenIndex(index, token.Length);
        }

        private static int RequireNonNegative(this int index)
        {
            Require.That(() => index != -1, "String does not contain requested token");
            return index;
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
            int i = 0;
            while (i < value.Length && predicate(value[i]))
            {
                ++i;
            }
            return value.Substring(0, i);
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

        public static string ToCsvWithASpace(this IEnumerable<string> items)
        {
            return string.Join(", ", items);
        }

        public static string ToMultiLine(this IEnumerable<string> items)
        {
            return string.Join(Environment.NewLine, items);
        }
    }
}
