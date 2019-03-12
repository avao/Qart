using Newtonsoft.Json.Linq;
using Qart.Core.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Diff
{
    public class JsonPatchCreator
    {
        public static void Compare(JToken lhs, JToken rhs, IDiffReporter diffReporter, IIdProvider idProvider)
        {
            Compare(lhs, rhs, Enumerable.Empty<string>(), diffReporter, idProvider);
        }

        public static void Compare(JToken lhs, JToken rhs, IEnumerable<string> path, IDiffReporter diffReporter, IIdProvider idProvider)
        {
            if (rhs == null)
            {
                if (lhs != null)
                    diffReporter.OnRemoved(path, lhs);
                return;
            }

            switch (lhs)
            {
                case null:
                    if (rhs != null)
                    {
                        diffReporter.OnAdded(path, rhs);
                    }
                    break;

                case JObject lhsJobj:
                    if (rhs is JObject rhsJobj)
                    {
                        Compare(lhsJobj, rhsJobj, path, diffReporter, idProvider);
                    }
                    else
                    {
                        diffReporter.OnChanged(path, lhs, rhs);
                    }
                    break;
                case JArray lhsArray:
                    if (rhs is JArray rhsArray)
                    {
                        Compare(lhsArray, rhsArray, path, diffReporter, idProvider);
                    }
                    else
                    {
                        diffReporter.OnChanged(path, lhs, rhs);
                    }
                    break;
                default:
                    CompareAtomic(lhs, rhs, path, diffReporter, idProvider);
                    break;
            }
        }

        private bool IsNull(JToken token)
        {
            return token == null || token.Type == JTokenType.Null;
        }

        private static void Compare(JObject lhs, JObject rhs, IEnumerable<string> path, IDiffReporter diffReporter, IIdProvider idProvider)
        {
            var lhsOrderedKeys = lhs.Properties().Select(p => p.Name).OrderBy(_ => _);
            var rhsOrderedKeys = rhs.Properties().Select(p => p.Name).OrderBy(_ => _);

            foreach (var pair in lhsOrderedKeys.JoinWithNulls(rhsOrderedKeys))
            {
                if (pair.Item1 == null)
                {
                    diffReporter.OnAdded(path.Concat(new[] { pair.Item2 }), rhs[pair.Item2]);
                }
                else if (pair.Item2 == null)
                {
                    diffReporter.OnRemoved(path.Concat(new[] { pair.Item1 }), lhs[pair.Item1]);
                }
                else
                {
                    Compare(lhs[pair.Item1], rhs[pair.Item2], path.Concat(new[] { pair.Item1 }), diffReporter, idProvider);
                }
            }
        }

        private static void Compare(JArray lhs, JArray rhs, IEnumerable<string> path, IDiffReporter diffReporter, IIdProvider idProvider)
        {
            var lhsElements = IdentifyElements(lhs, path, idProvider);
            var rhsElements = IdentifyElements(rhs, path, idProvider);

            var lhsOrderedKeys = lhsElements.Keys.OrderBy(_ => _).ToList();
            var rhsOrderedKeys = rhsElements.Keys.OrderBy(_ => _).ToList();

            foreach (var pair in lhsOrderedKeys.JoinWithNulls(rhsOrderedKeys))
            {
                if (pair.Item1 == null)
                {
                    diffReporter.OnAdded(path.Concat(new[] { pair.Item2 }), rhsElements[pair.Item2]);
                }
                else if (pair.Item2 == null)
                {
                    diffReporter.OnRemoved(path.Concat(new[] { pair.Item1 }), lhsElements[pair.Item1]);
                }
                else
                {
                    Compare(lhsElements[pair.Item1], rhsElements[pair.Item2], path.Concat(new[] { pair.Item1 }), diffReporter, idProvider);
                }
            }
        }
        

        private static IDictionary<string, JToken> IdentifyElements(JArray array, IEnumerable<string> path, IIdProvider idProvider)
        {
            return array.Select((item, index) => (idProvider.GetId(path, item, index), item)).ToDictionary(kvp => kvp.Item1, kvp => kvp.Item2);
        }

        private static void CompareAtomic(JToken lhs, JToken rhs, IEnumerable<string> path, IDiffReporter diffReporter, IIdProvider idProvider)
        {
            if (lhs.Type != rhs.Type || lhs.ToString() != rhs.ToString()) //TODO compare type wise?
            {
                diffReporter.OnChanged(path, lhs, rhs);
            }
        }
    }
}
