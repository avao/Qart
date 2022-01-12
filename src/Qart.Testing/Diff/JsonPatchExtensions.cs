using Newtonsoft.Json.Linq;
using Qart.Core.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Diff
{
    public static class JsonPatchExtensions
    {
        public static void ApplyPatch(this JToken inputToken, IEnumerable<DiffItem> diffs)
        {
            var actions = new List<Action>();
            foreach (DiffItem diff in diffs)
            {
                var tokens = inputToken.SelectTokens(diff.JsonPath)
                    .ToList();

                if (tokens.Count == 0)
                {
                    actions.Add(() => inputToken.AddTokenByJsonPath(diff.JsonPath, diff.Value));
                }
                else
                {
                    actions.AddRange(tokens.Select<JToken, Action>(token => (diff.Value?.Type ?? JTokenType.Null) == JTokenType.Null
                                                                                ? () => Remove(token)
                                                                                : () => token.Replace(diff.Value)));
                }
            }

            actions.ForEach(action => action.Invoke());
        }

        public static void RemoveTokens(this JToken token, IEnumerable<string> jsonPaths)
        {
            jsonPaths.SelectMany(jsonPath => token.SelectTokens(jsonPath).Select<JToken, Action>(t => () => Remove(t)))
                     .ToList() //force iteration before updates
                     .ForEach(action => action.Invoke());
        }

        public static void AddTokenByJsonPath(this JToken token, string jsonPath, JToken childToken)
        {
            //TODO not implemented! Just a hack for single trailing property
            if (jsonPath.EndsWith("]"))
            {//selector like ...items[?(@.prop='blah')]
                var jpath = jsonPath.LeftOfLast("[");
                var parentToken = (JArray)token.SelectToken(jpath);
                parentToken.Add(childToken);
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

        private static void Remove(this JToken tokenToRemove)
        {
            var adjustedToken = tokenToRemove?.Parent is JProperty jProperty ? jProperty : tokenToRemove;
            adjustedToken.Remove();
        }
    }
}
