using Qart.Testing.Context;
using System.Collections;
using System.Collections.Generic;

namespace Qart.Testing.ActionPipeline
{
    public class ItemsHolder : IItemsHolder
    {
        private readonly IDictionary<string, object> _items;

        public ItemsHolder()
        {
            _items = new Dictionary<string, object>();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void SetItem<T>(string key, T item)
        {
            _items[key] = item;
        }

        public bool TryGetItem<T>(string key, out T item)
        {
            if (_items.TryGetValue(key, out var obj))
            {
                item = (T)obj;
                return true;
            }

            item = default;
            return false;
        }

        public bool TryRemoveItem(string key)
        {
            return _items.Remove(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
