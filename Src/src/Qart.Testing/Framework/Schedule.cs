using Qart.Core.Collections;
using Qart.Testing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Qart.Testing.Framework
{
    public class Schedule<T> : ISchedule<T>
    {
        private object _queueLock;
        private object _tokensLock;
        private readonly List<Tuple<T, IEnumerable<string>>> _queue;
        private readonly ConcurrentDictionary<T, IEnumerable<string>> _acquired;
        private readonly ConcurrentDictionary<string, int> _lockedTokens;
        private readonly ICriticalSectionTokensProvider<T> _csTokenProvider;


        //TODO sorting?
        public Schedule(ICriticalSectionTokensProvider<T> csTokenProvider)
        {
            _queueLock = new object();
            _tokensLock = new object();
            _csTokenProvider = csTokenProvider;
            _queue = new List<Tuple<T, IEnumerable<string>>>();
            _acquired = new ConcurrentDictionary<T, IEnumerable<string>>();
            _lockedTokens = new ConcurrentDictionary<string, int>();
        }

        public void Enqueue(T item)
        {
            lock (_queueLock)
            {
                _queue.Add(new Tuple<T, IEnumerable<string>>(item, _csTokenProvider.GetTokens(item)));
            }
        }

        private bool TryAcquireForProcessing(string workerId, out T task)
        {
            lock (_queueLock)
            {
                var item = _queue.FirstOrDefault(_ => IsAllowedToExecute(_.Item2));
                if (item != null)
                {
                    _queue.Remove(item);
                    if (!_acquired.TryAdd(item.Item1, item.Item2))
                        throw new Exception("Could not add task to acquired");
                    UpdateTokens(item.Item2, +1);

                    task = item.Item1;
                    return true;
                }
                task = default(T);
                return false;
            }
        }

        public T AcquireForProcessing(string workerId)
        {
            while(true)
            {
                T task;
                if (TryAcquireForProcessing(workerId, out task))
                {
                    return task;
                }
                else if(_queue.Count>0)
                {
                    Thread.Sleep(100);//TODO wait for an event
                }
                else
                {
                    return default(T);
                }
            }
        }

        public void Dequeue(T task)
        {
            IEnumerable<string> tokens;
            if (_acquired.TryRemove(task, out tokens))
            {
                UpdateTokens(tokens, -1);
            }
            else
            {
                throw new Exception("Could not dequeue not acquired task");
            }
        }

        private bool IsAllowedToExecute(IEnumerable<string> tokens)
        {
            return !tokens.Any(_ => _lockedTokens.GetOptionalValue(_, 0) > 0);
        }


        private void UpdateTokens(IEnumerable<string> tokens, int delta)
        {
            lock (_tokensLock)
            {
                foreach (var token in tokens)
                {
                    int count;
                    if (!_lockedTokens.TryGetValue(token, out count))
                    {
                        if (delta < 0)
                            throw new Exception("Cannot update token " + token);
                    }
                    _lockedTokens[token] = count + delta;
                }
            }
        }
    }
}
