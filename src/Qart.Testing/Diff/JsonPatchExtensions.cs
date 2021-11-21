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
            diffs.SelectMany(diff => inputToken.SelectTokens(diff.JsonPath).Select(token => (diff, token)))
                 .Select(tuple => (tuple.diff.Value?.Type ?? JTokenType.Null) == JTokenType.Null
                                    ? tuple.token.RemovalAction()
                                    : tuple.token.CreateUpsertAction(tuple.diff))
                .Apply();
        }

        public static void RemoveTokens(this JToken token, IEnumerable<string> jsonPaths)
        {
            jsonPaths.SelectMany(jsonPath => token.SelectTokens(jsonPath).Select(RemovalAction))
                     .Apply();
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


        private static Action RemovalAction(this JToken tokenToRemove)
        {
            return (tokenToRemove.Parent is JProperty jProperty
                            ? jProperty
                            : tokenToRemove)
                         .Remove;
        }

        private static Action CreateUpsertAction(this JToken t, DiffItem diff)
        {
            return t == null
                    ? () => t.AddTokenByJsonPath(diff.JsonPath, diff.Value)
                    : () => t.Replace(diff.Value);
        }

        private static void Apply(this IEnumerable<Action> actions)
        {
            actions.ToList() //force iteration before updates
                   .ForEach(action => action.Invoke());
        }
    }
}
