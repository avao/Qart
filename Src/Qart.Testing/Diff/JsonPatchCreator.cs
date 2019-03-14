using Newtonsoft.Json.Linq;
using Qart.Core.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Diff
{
    public class JsonPatchCreator
    {
        public static IEnumerable<DiffItem> Compare(JToken lhs, JToken rhs, IIdProvider idProvider)
        {
            return Compare(lhs, rhs, Enumerable.Empty<string>(), idProvider);
        }

        private static IEnumerable<DiffItem> Compare(JToken lhs, JToken rhs, IEnumerable<string> path, IIdProvider idProvider)
        {
            if (rhs == null)
            {
                if (lhs != null)
                {
                    yield return new DiffItem(path, lhs, null);
                }
                yield break;
            }

            switch (lhs)
            {
                case null:
                    if (rhs != null)
                    {
                        yield return new DiffItem(path, null, rhs);
                    }
                    break;
                case JObject lhsJobj:
                    if (rhs is JObject rhsJobj)
                    {
                        var lhsOrderedKeys = lhsJobj.Properties().Select(p => p.Name);
                        var rhsOrderedKeys = rhsJobj.Properties().Select(p => p.Name);
                        foreach (var diff in CompareChildren(path, lhsOrderedKeys, rhsOrderedKeys, lhsJobj, rhsJobj, idProvider))
                        {
                            yield return diff;
                        }
                    }
                    else
                    {
                        yield return new DiffItem(path, lhs, rhs);
                    }
                    break;
                case JArray lhsArray:
                    if (rhs is JArray rhsArray)
                    {
                        var lhsElements = IdentifyElements(lhsArray, path, idProvider);
                        var rhsElements = IdentifyElements(rhsArray, path, idProvider);
                        foreach (var diff in CompareChildren(path, lhsElements.Keys, rhsElements.Keys, lhsElements, rhsElements, idProvider))
                        {
                            yield return diff;
                        }
                    }
                    else
                    {
                        yield return new DiffItem(path, lhs, rhs);
                    }
                    break;
                default:
                    if (lhs.Type != rhs.Type || lhs.ToString() != rhs.ToString()) //TODO compare type wise?
                    {
                        yield return new DiffItem(path, lhs, rhs);
                    }
                    break;
            }
        }

        private static IEnumerable<DiffItem> CompareChildren(IEnumerable<string> path, IEnumerable<string> lhsKeys, IEnumerable<string> rhsKeys, IDictionary<string, JToken> lhsElements, IDictionary<string, JToken> rhsElements, IIdProvider idProvider)
        {
            foreach ((string lhsKey, string rhsKey) in lhsKeys.OrderBy(_ => _).JoinWithNulls(rhsKeys.OrderBy(_ => _)))
            {
                if (lhsKey == null)
                {
                    yield return new DiffItem(path.Concat(new[] { rhsKey }), null, rhsElements[rhsKey]);
                }
                else if (rhsKey == null)
                {
                    yield return new DiffItem(path.Concat(new[] { lhsKey }), lhsElements[lhsKey], null);
                }
                else
                {
                    foreach (var diff in Compare(lhsElements[lhsKey], rhsElements[rhsKey], path.Concat(new[] { lhsKey }), idProvider))
                    {
                        yield return diff;
                    }
                }
            }
        }

        private static IDictionary<string, JToken> IdentifyElements(JArray array, IEnumerable<string> path, IIdProvider idProvider)
        {
            return array.Select((item, index) => (idProvider.GetId(path, item, index), item)).ToDictionary(kvp => kvp.Item1, kvp => kvp.Item2);
        }
    }
}
