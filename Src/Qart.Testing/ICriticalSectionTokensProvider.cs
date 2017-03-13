using Qart.Testing.Framework;
using System.Collections.Generic;

namespace Qart.Testing
{
    public interface ICriticalSectionTokensProvider<T>
    {
        IEnumerable<string> GetTokens(T testCase);
    }

    public interface IOrderer<T>
    {
        IEnumerable<T> Order(IEnumerable<T> items);
    }

    public interface ISchedule<T>
    {
        void Enqueue(T item);
        T AcquireForProcessing(string workerId);
        void Dequeue(T item);
    }


    public static class ScheduleExtensions
    {
        public static void Enqueue<T>(this ISchedule<T> schedule, IEnumerable<T> tasks)
        {
            foreach (var task in tasks)
            {
                schedule.Enqueue(task);
            }
        }
    }
}
