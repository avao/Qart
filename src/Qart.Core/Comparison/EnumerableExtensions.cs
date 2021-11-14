using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Comparison
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T, T)> JoinWithNulls<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
            => lhs.JoinWithNulls(rhs, item => item);

        public static IEnumerable<(T, T)> JoinWithNulls<T, TKey>(this IEnumerable<T> lhs, IEnumerable<T> rhs, Func<T, TKey> keySelector)
            => lhs.JoinWithNulls(rhs, keySelector, Comparer<TKey>.Default);

        public static IEnumerable<(T, T)> JoinWithNulls<T, TKey>(this IEnumerable<T> lhs, IEnumerable<T> rhs, Func<T, TKey> keySelector, IComparer<TKey> keyComparer)
            => lhs.JoinWithNulls(rhs, keySelector, keySelector, keyComparer);

        public static IEnumerable<(T1, T2)> JoinWithNulls<T1, T2, TKey>(this IEnumerable<T1> lhs, IEnumerable<T2> rhs, Func<T1, TKey> keySelector1, Func<T2, TKey> keySelector2, IComparer<TKey> keyComparer)
        {
            using var lhsEnumerator = lhs.OrderBy(keySelector1, keyComparer).GetEnumerator();
            using var rhsEnumerator = rhs.OrderBy(keySelector2, keyComparer).GetEnumerator();

            bool lhsMoved = lhsEnumerator.MoveNext();
            bool rhsMoved = rhsEnumerator.MoveNext();
            while (lhsMoved && rhsMoved)
            {
                int compareResult = keyComparer.Compare(keySelector1.Invoke(lhsEnumerator.Current), keySelector2.Invoke(rhsEnumerator.Current));
                if (compareResult == 0)
                {
                    yield return (lhsEnumerator.Current, rhsEnumerator.Current);
                    lhsMoved = lhsEnumerator.MoveNext();
                    rhsMoved = rhsEnumerator.MoveNext();
                }
                else if (compareResult < 0)
                {
                    yield return (lhsEnumerator.Current, default(T2));
                    lhsMoved = lhsEnumerator.MoveNext();
                }
                else if (compareResult > 0)
                {
                    yield return (default(T1), rhsEnumerator.Current);
                    rhsMoved = rhsEnumerator.MoveNext();
                }
            }

            while (lhsMoved)
            {
                yield return (lhsEnumerator.Current, default(T2));
                lhsMoved = lhsEnumerator.MoveNext();
            }

            while (rhsMoved)
            {
                yield return (default(T1), rhsEnumerator.Current);
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
