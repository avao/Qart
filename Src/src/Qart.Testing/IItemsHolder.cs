using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Core.DataStore;
using Qart.Core.Text;
using Qart.Core.Validation;
using System;
using System.Collections.Generic;

namespace Qart.Testing
{
    public interface IItemsHolder : IEnumerable<KeyValuePair<string, object>>
    {
        void SetItem<T>(string key, T item);
        bool TryGetItem<T>(string key, out T item) where T : class;
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
                Require.Fail($"Could not get required item {key}");
            }

            return item;
        }

        public static string GetRequiredItem(this IItemsHolder pipelineContext, string key)
        {
            var item = pipelineContext.GetRequiredItem<object>(key);
            switch (item)
            {
                case null:
                    return null;
                case string stringValue:
                    return stringValue;
                case JToken jtokenValue:
                    return JsonConvert.SerializeObject(jtokenValue);
                default:
                    throw new NotSupportedException($"Unsupported item type {item.GetType()} for conversion into string");
            }
        }

        public static JToken GetRequiredItemAsJToken(this IItemsHolder pipelineContext, string key)
        {
            var item = pipelineContext.GetRequiredItem<object>(key);
            switch (item)
            {
                case null:
                    return null;
                case string stringValue:
                    return JToken.Parse(stringValue);
                case JToken jtokenValue:
                    return jtokenValue;
                case object obj:
                    return JObject.FromObject(obj);
                default:
                    throw new NotSupportedException($"Unsupported item type {item.GetType()} for conversion into JToken");
            }
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
    }
}
