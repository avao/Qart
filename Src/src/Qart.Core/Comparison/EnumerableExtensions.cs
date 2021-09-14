using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Comparison
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T, T)> JoinWithNulls<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
            where T : class
        {
            return lhs.JoinWithNulls(rhs, Comparer<T>.Default, item => item);
        }

        public static IEnumerable<(T, T)> JoinWithNulls<T, TKey>(this IEnumerable<T> lhs, IEnumerable<T> rhs, IComparer<T> comparer, Func<T, TKey> keySelector)
            where T : class
        {
            return lhs.JoinWithNulls(rhs, (l, r) => comparer.Compare(l, r), keySelector);
        }

        public static IEnumerable<(T, T)> JoinWithNulls<T, TKey>(this IEnumerable<T> lhs, IEnumerable<T> rhs, Func<T, T, int> comparerFunc, Func<T, TKey> keySelector)
            where T : class
        {
            return lhs.OrderBy(keySelector).JoinWithNulls(rhs.OrderBy(keySelector), comparerFunc);
        }

        public static IEnumerable<(T, T)> JoinWithNulls<T>(this IOrderedEnumerable<T> lhs, IOrderedEnumerable<T> rhs, Func<T, T, int> comparerFunc)
            where T : class
        {
            using var lhsEnumerator = lhs.GetEnumerator();
            using var rhsEnumerator = rhs.GetEnumerator();

            bool lhsMoved = lhsEnumerator.MoveNext();
            bool rhsMoved = rhsEnumerator.MoveNext();
            while (lhsMoved && rhsMoved)
            {
                int compareResult = comparerFunc.Invoke(lhsEnumerator.Current, rhsEnumerator.Current);
                if (compareResult == 0)
                {
                    yield return (lhsEnumerator.Current, rhsEnumerator.Current);
                    lhsMoved = lhsEnumerator.MoveNext();
                    rhsMoved = rhsEnumerator.MoveNext();
                }
                else if (compareResult < 0)
                {
                    yield return (lhsEnumerator.Current, null);
                    lhsMoved = lhsEnumerator.MoveNext();
                }
                else if (compareResult > 0)
                {
                    yield return (null, rhsEnumerator.Current);
                    rhsMoved = rhsEnumerator.MoveNext();
                }
            }

            while (lhsMoved)
            {
                yield return (lhsEnumerator.Current, null);
                lhsMoved = lhsEnumerator.MoveNext();
            }

            while (rhsMoved)
            {
                yield return (null, rhsEnumerator.Current);
                rhsMoved = rhsEnumerator.MoveNext();
            }
        }

        public static bool IsEqualTo<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs, IEqualityComparer<T> comparer)
        {
            using (var lhsEnumerator = lhs.GetEnumerator())
            using (var rhsEnumerator = rhs.GetEnumerator())
            {
                bool lhsMoved = lhsEnumerator.MoveNext();
                bool rhsMoved = rhsEnumerator.MoveNext();

                while (lhsMoved && rhsMoved)
                {
                    if (!comparer.Equals(lhsEnumerator.Current, rhsEnumerator.Current))
                    {
                        return false;
                    }
                    lhsMoved = lhsEnumerator.MoveNext();
                    rhsMoved = rhsEnumerator.MoveNext();
                }

                if (lhsMoved ^ rhsMoved)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
