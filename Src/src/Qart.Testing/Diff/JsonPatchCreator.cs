using Newtonsoft.Json.Linq;
using Qart.Core.Comparison;
using Qart.Testing.Framework.Json;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Diff
{
    public class JsonPatchCreator
    {
        public static IEnumerable<DiffItem> Compare(JToken lhs, JToken rhs, ITokenSelectorProvider idProvider)
        {
            return Compare(lhs, rhs, "$", idProvider);
        }

        private static IEnumerable<DiffItem> Compare(JToken lhs, JToken rhs, string jsonPath, ITokenSelectorProvider idProvider)
        {
            if (rhs == null)
            {
                if (lhs != null)
                {
                    yield return new DiffItem(jsonPath, null);
                }
                yield break;
            }

            switch (lhs)
            {
                case null:
                    if (rhs != null)
                    {
                        yield return new DiffItem(jsonPath, rhs);
                    }
                    break;
                case JObject lhsJobj:
                    if (rhs is JObject rhsJobj)
                    {
                        var lhsOrderedKeys = lhsJobj.Properties().Select(p => p.Name);
                        var rhsOrderedKeys = rhsJobj.Properties().Select(p => p.Name);
                        foreach (var diff in CompareChildren(jsonPath, lhsOrderedKeys, rhsOrderedKeys, lhsJobj, rhsJobj, idProvider))
                        {
                            yield return diff;
                        }
                    }
                    else
                    {
                        yield return new DiffItem(jsonPath, rhs);
                    }
                    break;
                case JArray lhsArray:
                    if (rhs is JArray rhsArray)
                    {
                        var lhsElements = IdentifyElements(lhsArray, jsonPath, idProvider);
                        var rhsElements = IdentifyElements(rhsArray, jsonPath, idProvider);
                        foreach (var diff in CompareChildren(jsonPath, lhsElements.Keys, rhsElements.Keys, lhsElements, rhsElements, idProvider))
                        {
                            yield return diff;
                        }
                    }
                    else
                    {
                        yield return new DiffItem(jsonPath, rhs);
                    }
                    break;
                default:
                    if (lhs.Type != rhs.Type || lhs.ToString() != rhs.ToString()) //TODO compare type wise?
                    {
                        yield return new DiffItem(jsonPath, rhs);
                    }
                    break;
            }
        }

        private static IEnumerable<DiffItem> CompareChildren(string jsonPath, IEnumerable<string> lhsKeys, IEnumerable<string> rhsKeys, IDictionary<string, JToken> lhsElements, IDictionary<string, JToken> rhsElements, ITokenSelectorProvider idProvider)
        {
            foreach ((string lhsKey, string rhsKey) in lhsKeys.JoinWithNulls(rhsKeys))
            {
                if (lhsKey == null)
                {
                    yield return new DiffItem(JsonPathFormatter.AddToken(jsonPath, rhsKey), rhsElements[rhsKey]);
                }
                else if (rhsKey == null)
                {
                    yield return new DiffItem(JsonPathFormatter.AddToken(jsonPath, lhsKey), null);
                }
                else
                {
                    foreach (var diff in Compare(lhsElements[lhsKey], rhsElements[rhsKey], JsonPathFormatter.AddToken(jsonPath, lhsKey), idProvider))
                    {
                        yield return diff;
                    }
                }
            }
        }

        private static IDictionary<string, JToken> IdentifyElements(JArray array, string jsonPath, ITokenSelectorProvider idProvider)
        {
            return array.Select((item, index) => (idProvider.GetTokenSelector(jsonPath, item, index), item)).ToDictionary(kvp => kvp.Item1, kvp => kvp.Item2);
        }
    }
}
