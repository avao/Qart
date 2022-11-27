using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Testing.Framework.Json
{
    public static class JsonExtensions
    {
        public static void OrderItems<TKey>(this JArray array, Func<JToken, TKey> keySelector)
        {
            var orderedArray = new JArray();
            foreach (var item in array.Children().OrderBy(keySelector))
            {
                orderedArray.Add(item);
            }

            array.ReplaceAll(orderedArray.Children());
        }

        public static void Order<TKey>(this JObject obj, string path, Func<JToken, TKey> keySelector)
        {
            foreach (var array in obj.SelectTokens(path).OfType<JArray>())
            {
                array.OrderItems(keySelector);
            }
        }

        public static void Order(this JObject obj, string path, IReadOnlyCollection<string> keyPaths)
        {
            if (keyPaths.Count == 1)
            {
                obj.Order(path, token => token.SelectToken(keyPaths.First()));
            }
            else
            {
                obj.Order(path, token => keyPaths.Select(_ => token.SelectToken(_)).ToArray());//TODO check comparison, what if many Tokens?
            }
        }

        public static string ToIndentedJson(this object o)
        {
            return JsonConvert.SerializeObject(o, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        public static string ToIndentedJsonNoDefaults(this object o)
        {
            return JsonConvert.SerializeObject(o, new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
        }
    }
}
