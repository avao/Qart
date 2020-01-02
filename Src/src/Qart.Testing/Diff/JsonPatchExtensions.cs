using Newtonsoft.Json.Linq;
using Qart.Core.Text;
using System;
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
                    if (tokenToChange == null)
                    {
                        AddTokenByJsonPath(token, diff.JsonPath, diff.Value);
                    }
                    else
                    {
                        tokenToChange.Replace(diff.Value);
                    }
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

        public static void AddTokenByJsonPath(this JToken token, string jsonPath, JToken childToken)
        {
            //TODO not implemented! Just a hack for single trailing property
            if(jsonPath.EndsWith("]"))
            {//selector like ...items[?(@.prop='blah')]
                var jpath = jsonPath.LeftOfLast("[");
                var parentToken = (JArray)token.SelectToken(jpath);
            }
            else
            {
                (string left, string right) = jsonPath.SplitOnLast(".");
                var parentToken = token.SelectToken(left);
                switch (parentToken)
                {
                    case JObject jObject:
                        jObject.Add(right, childToken);
                        break;
                    default:
                        throw new NotSupportedException($"{nameof(AddTokenByJsonPath)} does not support non-object parent tokens");
                }
            }
        }
    }
}
