using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Framework.Json
{
    public static class JsonPathFormatter
    {
        public static string AddToken(string currentPath, string item)
        {
            return item.StartsWith("[")
                ? currentPath + item
                : $"{currentPath}.{item}";
        }

        public static string FormatIndex(int index)
        {
            return $"[{index.ToString()}]";
        }

        public static string FormatAndCondition(IEnumerable<(string property, string value)> conditions)
        {
            var conditionsString = string.Join(" && ", conditions.Select(condition => $"@{condition.property}=={condition.value}"));
            return $"[?({conditionsString})]";
        }

        public static string FormatAndCondition(IEnumerable<(string property, JToken token)> conditions)
        {
            return FormatAndCondition(conditions.Select(pair => (pair.property, TokenValueToString(pair.token))));
        }

        private static string TokenValueToString(JToken token)
        {
            var value = token.ToString();
            if (token.Type == JTokenType.String)
            {
                value = $"'{value}'";
            }
            return value;
        }
    }
}
