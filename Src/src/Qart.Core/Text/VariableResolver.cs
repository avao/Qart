using Qart.Core.Validation;
using System;
using System.Text;

namespace Qart.Core.Text
{
    public static class VariableResolver
    {
        public static string Resolve(string value, Func<string, string> resolverFunc)
        {
            var dollarPos = value.IndexOf('$');
            if (dollarPos == -1)
                return value;

            int startIndex = 0;
            var stringBuilder = new StringBuilder();
            while (dollarPos != -1)
            {
                var openingCurlyPos = dollarPos + 1;
                if (value.Length > openingCurlyPos && value[openingCurlyPos] == '{')
                {
                    var closingCurlyPos = value.IndexOf('}', openingCurlyPos);
                    Require.That(() => closingCurlyPos != -1, $"Could not find matching '}}' for an '{{' at position {openingCurlyPos}.");

                    var variableName = value.Substring(openingCurlyPos + 1, closingCurlyPos - openingCurlyPos - 1);
                    var variableValue = resolverFunc(variableName);

                    stringBuilder.Append(value.Substring(startIndex, dollarPos - startIndex));
                    stringBuilder.Append(Resolve(variableValue, resolverFunc));
                    startIndex = closingCurlyPos + 1;
                }
                else
                {
                    stringBuilder.Append(value.Substring(startIndex, dollarPos - startIndex + 1));
                    startIndex = dollarPos + 1;
                }

                dollarPos = value.IndexOf('$', startIndex);
            }
            stringBuilder.Append(value.Substring(startIndex));

            return stringBuilder.ToString();
        }
    }
}
