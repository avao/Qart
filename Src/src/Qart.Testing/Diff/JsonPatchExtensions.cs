using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Diff
{
    public static class JsonPatchExtensions
    {
        public static void ApplyPatch(this JToken token, IEnumerable<DiffItem> diffs)
        {
            foreach (var diff in diffs)
            {
                if (diff.Value == null || diff.Value.Type == JTokenType.Null)
                {
                    token.RemoveTokens(diff.JsonPath);
                }
                else
                {
                    var tokenToChange = token.SelectToken(diff.JsonPath);
                    tokenToChange.Replace(diff.Value);
                }
            }
        }

        public static void RemoveTokens(this JToken token, string jsonPath)
        {
            foreach (var tokenToRemove in token.SelectTokens(jsonPath).ToList())
            {
                if (tokenToRemove.Parent is JProperty jProperty)
                {
                    jProperty.Remove();
                }
                else
                {
                    tokenToRemove.Remove();
                }
            }
        }

        public static void RemoveTokens(this JToken token, IEnumerable<string> jsonPaths)
        {
            foreach (var jsonPath in jsonPaths)
            {
                token.RemoveTokens(jsonPath);
            }
        }
    }
}
