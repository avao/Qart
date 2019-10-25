using Qart.Testing.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Qart.Testing.ActionPipeline
{
    public class ItemsHolder : IItemsHolder
    {
        private readonly IDictionary<string, object> _items;
        private readonly IItemProvider _itemsInitialiser;

        public ItemsHolder(IItemProvider itemsInitialiser)
        {
            _items = new Dictionary<string, object>();
            _itemsInitialiser = itemsInitialiser;
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
            where T : class
        {
            if (_items.TryGetValue(key, out var obj))
            {
                item = (T)obj;
                return true;
            }

            if (_itemsInitialiser.TryGetItem<T>(key, out item))
            {
                SetItem(key, item);
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
