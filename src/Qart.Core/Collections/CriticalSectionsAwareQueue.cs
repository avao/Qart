using Qart.Core.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Collections
{
    public class CriticalSectionsAwareQueue<T>
    {
        private readonly object _queueLock;
        private readonly object _tokensLock;
        private readonly List<(T Item, IReadOnlyCollection<string> Tokens)> _queue;
        private readonly ConcurrentDictionary<T, IReadOnlyCollection<string>> _acquired;
        private readonly ConcurrentDictionary<string, int> _lockedTokens;

        public CriticalSectionsAwareQueue()
        {
            _queueLock = new object();
            _tokensLock = new object();
            _queue = new List<(T, IReadOnlyCollection<string>)>();
            _acquired = new ConcurrentDictionary<T, IReadOnlyCollection<string>>();
            _lockedTokens = new ConcurrentDictionary<string, int>();
        }

        public void Enqueue(T item, IReadOnlyCollection<string> criticalSections)
        {
            lock (_queueLock)
            {
                _queue.Add((item, criticalSections));
            }
        }

        public bool TryAcquireForProcessing(out T item, out int queueDepth)
        {
            lock (_queueLock)
            {                
                foreach (var kvp in _queue)
                {
                    if (kvp.Tokens.All(token => _lockedTokens.GetOptionalValue(token, 0) == 0))
                    {
                        _queue.Remove(kvp);
                        if (!_acquired.TryAdd(kvp.Item, kvp.Tokens))
                            throw new Exception("Could not add task to acquired");
                        UpdateTokens(kvp.Tokens, +1);

                        queueDepth = _queue.Count;
                        item = kvp.Item;
                        return true;
                    }
                }
                queueDepth = _queue.Count;
            }
            item = default;
            return false;
        }

        public void Dequeue(T item)
        {
            var removed = _acquired.TryRemove(item, out var tokens);
            Require.True(removed, "Could not dequeue not acquired task");
            UpdateTokens(tokens, -1);
        }

        private void UpdateTokens(IReadOnlyCollection<string> tokens, int delta)
        {
            lock (_tokensLock)
            {
                foreach (var token in tokens)
                {
                    _lockedTokens.TryGetValue(token, out int count);
                    var updatedValue = count + delta;
                    Require.That(() => updatedValue >= 0, $"Cannot update token {token}");
                    _lockedTokens[token] = updatedValue;
                }
            }
        }
    }
}
