using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Collections
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T, T)> JoinWithNulls<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
            where T : class
        {
            return lhs.JoinWithNulls(rhs, Comparer<T>.Default);
        }

        public static IEnumerable<(T, T)> JoinWithNulls<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs, IComparer<T> comparer)
            where T : class
        {
            using (var lhsEnumerator = lhs.GetEnumerator())
            using (var rhsEnumerator = rhs.GetEnumerator())
            {
                bool lhsMoved = lhsEnumerator.MoveNext();
                bool rhsMoved = rhsEnumerator.MoveNext();

                while (lhsMoved && rhsMoved)
                {
                    int compareResult = comparer.Compare(lhsEnumerator.Current, rhsEnumerator.Current);
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

                if (lhsMoved)
                {
                    do
                    {
                        yield return (lhsEnumerator.Current, null);
                        lhsMoved = lhsEnumerator.MoveNext();
                    } while (lhsMoved);
                }

                if (rhsMoved)
                {
                    do
                    {
                        yield return (null, rhsEnumerator.Current);
                        rhsMoved = rhsEnumerator.MoveNext();
                    } while (rhsMoved);
                }
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

        public static IEnumerable<T> ToEmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }
    }
}
