using System;
using System.Collections.Generic;

namespace Qart.Core.Collections
{
    public static class DictionaryExtensions
    {
        public static T GetOptionalValue<K,T>(this IDictionary<K,T> dictionary, K key, T defaultValue)
        {
            T result;
            if(!dictionary.TryGetValue(key, out result))
            {
                return defaultValue;
            }
            return result;
        }

        public static V GetOptionalValue<K,T,V>(this IDictionary<K,T> dictionary, K key, V defaultValue, Func<T,V> convert)
        {
            T result;
            if (!dictionary.TryGetValue(key, out result))
            {
                return defaultValue;
            }
            return convert(result);
        }
    }
}
