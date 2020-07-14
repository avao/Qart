using Qart.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qart.Core.Text
{
    public static class VariableResolver
    {
        public static string Resolve(string value, IDictionary<string, string> tokens)
        {
            return Resolve(value, token => tokens[token]);
        }

        public static string Resolve(string value, Func<string, string> resolverFunc)
        {
            const char prefixToken = '$';
            const char openingToken = '{';
            const char closingToken = '}';

            var dollarPos = value.IndexOf(prefixToken);
            if (dollarPos == -1)
                return value;

            int startIndex = 0;
            var stringBuilder = new StringBuilder();
            while (dollarPos != -1)
            {
                var openingTokenPos = dollarPos + 1;
                if (value.Length > openingTokenPos && value[openingTokenPos] == openingToken)
                {
                    var closingTokenPos = IndexOfClosingToken(value, openingTokenPos, closingToken);
                    Require.That(() => closingTokenPos != -1, $"Could not find matching '{openingToken}' for an '{closingToken}' at position {openingTokenPos}.");

                    var variableName = value.Substring(openingTokenPos + 1, closingTokenPos - openingTokenPos - 1);
                    string resolvedValue;
                    if (variableName.Length > 1 && variableName[0] == prefixToken && variableName[1] == openingToken)
                    {
                        resolvedValue = variableName;
                    }
                    else
                    {
                        var variableValue = resolverFunc(variableName);
                        stringBuilder.Append(value.Substring(startIndex, dollarPos - startIndex));
                        resolvedValue = Resolve(variableValue, resolverFunc);
                    }
                    stringBuilder.Append(resolvedValue);

                    startIndex = closingTokenPos + 1;
                }
                else
                {
                    stringBuilder.Append(value.Substring(startIndex, dollarPos - startIndex + 1));
                    startIndex = dollarPos + 1;
                }

                dollarPos = value.IndexOf(prefixToken, startIndex);
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
