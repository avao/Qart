using Qart.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qart.Core.Text
{
    public static class VariableResolver
    {
        public static string Resolve(string value, IDictionary<string, string> tokens)
            => Resolve(value, token => tokens[token]);

        public static string Resolve(string value, Func<string, object> resolverFunc, IFormatProvider formatProvider = null)
            => Resolve(value, (token, format) => string.Format(formatProvider, "{0" + format + "}", resolverFunc(token)));

        public static string Resolve(string value, Func<string, string, string> resolverFunc)
        {
            const char prefixToken = '$';
            const char openingToken = '{';
            const char closingToken = '}';

            var prefixPos = value.IndexOf(prefixToken);
            if (prefixPos == -1)
                return value;

            int startIndex = 0;
            var stringBuilder = new StringBuilder();
            while (prefixPos != -1)
            {
                var openingTokenPos = prefixPos + 1;
                if (value.Length > openingTokenPos && value[openingTokenPos] == openingToken)
                {
                    var closingTokenPos = IndexOfClosingToken(value, openingTokenPos, closingToken);
                    Require.That(() => closingTokenPos != -1, $"Could not find matching '{openingToken}' for an '{closingToken}' at position {openingTokenPos}.");

                    var variableNameAndFormat = value.Substring(openingTokenPos + 1, closingTokenPos - openingTokenPos - 1);
                    string resolvedValue;
                    if (variableNameAndFormat.Length > 1 && variableNameAndFormat[0] == prefixToken && variableNameAndFormat[1] == openingToken)
                    {//handles escape sequence like ${${var}} turning it into ${var}
                        resolvedValue = variableNameAndFormat;
                    }
                    else
                    {
                        (var variableNameAndAligment, var format) = variableNameAndFormat.SplitOnFirstOptional(":");
                        (var variableName, var alignment) = variableNameAndAligment.SplitOnFirstOptional(",");

                        var variableValue = resolverFunc(variableName, variableNameAndFormat.Substring(variableName.Length));
                        stringBuilder.Append(value.Substring(startIndex, prefixPos - startIndex));
                        resolvedValue = Resolve(variableValue, resolverFunc);
                    }
                    stringBuilder.Append(resolvedValue);

                    startIndex = closingTokenPos + 1;
                }
                else
                {
                    stringBuilder.Append(value.Substring(startIndex, prefixPos - startIndex + 1));
                    startIndex = prefixPos + 1;
                }

                prefixPos = value.IndexOf(prefixToken, startIndex);
            }
            stringBuilder.Append(value.Substring(startIndex));

            return stringBuilder.ToString();
        }

        public static int IndexOfClosingToken(string value, int openingTokenPos, char closingToken)
        {
            var openingToken = value[openingTokenPos];
            var nesting = 0;
            for (int i = openingTokenPos + 1; i < value.Length; ++i)
            {
                char curChar = value[i];
                if (curChar == openingToken)
                {
                    ++nesting;
                }
                else if (curChar == closingToken)
                {
                    if (nesting == 0)
                        return i;
                    --nesting;
                }
            }
            return -1;
        }
    }
}
