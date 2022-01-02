using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Qart.Core.Validation;
using Qart.Testing.Framework;
using System;
using System.Collections.Generic;

namespace Qart.Testing.Context
{
    public interface IItemsHolder : IEnumerable<KeyValuePair<string, object>>
    {
        void SetItem<T>(string key, T item);
        bool TryGetItem<T>(string key, out T item);
        bool TryRemoveItem(string key);
    }

    public static class ItemsHolderExtensions
    {
        public static T GetItem<T>(this IItemsHolder pipelineContext, string key)
            where T : class
        {
            pipelineContext.TryGetItem<T>(key, out var item);
            return item;
        }

        public static T GetRequiredItem<T>(this IItemsHolder pipelineContext, string key)
            where T : class
        {
            if (!pipelineContext.TryGetItem<T>(key, out var item))
            {
                Require.Fail($"Could not get required item: {key}");
            }

            return item;
        }

        public static string GetRequiredItem(this IItemsHolder pipelineContext, string key)
        {
            var item = pipelineContext.GetRequiredItem<object>(key);
            return item switch
            {
                null => null,
                string stringValue => stringValue,
                JToken jtokenValue => JsonConvert.SerializeObject(jtokenValue),
                _ => throw new NotSupportedException($"Unsupported item type {item.GetType()} for conversion into string"),
            };
        }

        public static JToken GetRequiredItemAsJToken(this IItemsHolder pipelineContext, string key)
        {
            var item = pipelineContext.GetRequiredItem<object>(key);
            return item switch
            {
                null => null,
                string stringValue => JToken.Parse(stringValue),
                JToken jtokenValue => jtokenValue,
                object obj => JObject.FromObject(obj)
            };
        }

        public static string Resolve(this IItemsHolder pipelineContext, string value)
        {
            return VariableResolver.Resolve(value, (key) => pipelineContext.GetRequiredItem(key));
        }

        public static string GetRequiredResolvedContent(this IItemsHolder pipelineContext, TestCase testCase, string itemId)
        {
            var content = testCase.GetRequiredContent(itemId);
            return pipelineContext.Resolve(content);
        }

        public static T GetRequiredResolvedObject<T>(this IItemsHolder pipelineContext, TestCase testCase, string itemId)
        {
            var content = pipelineContext.GetRequiredResolvedContent(testCase, itemId);
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static void ReplaceTokens(this IItemsHolder pipelineContext, JToken doc, IDictionary<string, IReadOnlyCollection<string>> groups)
        {
            if (!pipelineContext.TryGetItem("TokenGroups", out IDictionary<string, IDictionary<string, int>> tokenGroups))
            {
                tokenGroups = new Dictionary<string, IDictionary<string, int>>();
                pipelineContext.SetItem("TokenGroups", tokenGroups);
            }

            Replace(doc, groups, new TokenHolder(tokenGroups));
        }

        public static void Replace(JToken doc, IDictionary<string, IReadOnlyCollection<string>> groups, IReplaceTokenHolder tokenMapper)
        {
            foreach (var kvp in groups)
            {
                foreach (var jsonPath in kvp.Value)
                {
                    foreach (var token in doc.SelectTokens(jsonPath))
                    {
                        var value = token.ToString();

                        value = tokenMapper.GetName(kvp.Key, jsonPath, value);

                        token.Replace(value);
                    }
                }
            }
        }
    }
}
