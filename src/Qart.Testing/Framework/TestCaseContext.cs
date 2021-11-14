using Qart.Testing.ActionPipeline;
using Qart.Testing.Context;
using Qart.Testing.Execution;
using Qart.Testing.Framework.Logging;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Qart.Testing.Framework
{
    public class TestCaseContext : IDisposable, IItemsHolder
    {
        public TestCase TestCase { get; }
        public IDisposableLogger Logger { get; }
        public IDescriptionWriter DescriptionWriter { get; }
        public IDictionary<string, string> Options { get; }

        private IItemsHolder _itemsHolder;

        public TestCaseContext(IDictionary<string, string> options, TestCase testCase, IDisposableLogger logger, IDescriptionWriter descriptionWriter, IItemsHolder itemsHolder)
        {
            Options = options;
            TestCase = testCase;
            Logger = logger;
            DescriptionWriter = descriptionWriter;
            _itemsHolder = itemsHolder;
        }

        public void Dispose()
        {
            Logger.Dispose();
        }


        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _itemsHolder.GetEnumerator();
        }

        public void SetItem<T>(string key, T item)
        {
            _itemsHolder.SetItem(GetEffectiveItemKey(key), item);
        }

        public bool TryGetItem<T>(string key, out T item)
        {
            return _itemsHolder.TryGetItem(GetEffectiveItemKey(key), out item);
        }

        public bool TryRemoveItem(string key)
        {
            return _itemsHolder.TryRemoveItem(GetEffectiveItemKey(key));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SetItemKey(string key)
        {
            SetItem(ItemKeys.ItemKey, key);
        }

        public string GetItemKey()
        {
            return _itemsHolder.GetItem<string>(ItemKeys.ItemKey) ?? string.Empty;
        }

        public string GetEffectiveItemKey(string key)
        {
            return key ?? _itemsHolder.GetItem<string>(ItemKeys.ItemKey) ?? string.Empty;
        }
    }
}
