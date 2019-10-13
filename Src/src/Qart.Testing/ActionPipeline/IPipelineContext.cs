using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qart.Core.Text;
using Qart.Core.Validation;
using System;
using System.Collections.Generic;

namespace Qart.Testing.ActionPipeline
{
    public interface IPipelineContext : IEnumerable<KeyValuePair<string, object>>
    {
        bool TryGetItem<T>(string key, out T item);
        void SetItem<T>(string key, T item);
        bool TryRemoveItem(string key);
    }

    public static class PipelineContextExtensions
    {
        public static T GetItem<T>(this IPipelineContext pipelineContext, string key)
        {
            pipelineContext.TryGetItem<T>(key, out var item);
            return item;
        }

        public static T GetRequiredItem<T>(this IPipelineContext pipelineContext, string key)
        {
            if (!pipelineContext.TryGetItem<T>(key, out var item))
            {
                Require.Fail($"Could not get required item {key}");
            }

            return item;
        }

        public static string GetRequiredItemAsString(this IPipelineContext pipelineContext, string key)
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

        public static JToken GetRequiredItemAsJToken(this IPipelineContext pipelineContext, string key)
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
                default:
                    throw new NotSupportedException($"Unsupported item type {item.GetType()} for conversion into JToken");
            }
        }

        public static string Resolve(this IPipelineContext pipelineContext, string value)
        {
            return VariableResolver.Resolve(value, (key) => pipelineContext.GetRequiredItemAsString(key));
        }
    }
}
