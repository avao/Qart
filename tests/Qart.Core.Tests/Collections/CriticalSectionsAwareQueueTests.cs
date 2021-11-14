using NUnit.Framework;
using Qart.Core.Collections;
using System;

namespace Qart.Core.Tests.Collections
{
    public class CriticalSectionsAwareQueueTests
    {
        [Test]
        public void CriticalSectionsAwareQueueTest()
        {
            var queue = new CriticalSectionsAwareQueue<int>();
            queue.Enqueue(1, new[] { "a", "b" });
            queue.Enqueue(2, new[] { "b" });
            queue.Enqueue(3, Array.Empty<string>());

            TryAcquireAndAssert(queue, true, 1, 2);
            TryAcquireAndAssert(queue, true, 3, 1);
            TryAcquireAndAssert(queue, false, default, 1);

            queue.Dequeue(1);

            TryAcquireAndAssert(queue, true, 2, 0);
        }

        private static void TryAcquireAndAssert<T>(CriticalSectionsAwareQueue<T> queue, bool isAquired, T expectedItem, int expectedQueueDepth)
        {
            Assert.That(queue.TryAcquireForProcessing(out var item, out var queueDepth), Is.EqualTo(isAquired));
            Assert.That(item, Is.EqualTo(expectedItem));
            Assert.That(queueDepth, Is.EqualTo(expectedQueueDepth));
        }
    }
}
