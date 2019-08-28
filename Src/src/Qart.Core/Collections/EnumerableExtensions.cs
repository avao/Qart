using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Collections
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T, T)> JoinWithNulls<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
        where T : class
        {
            using (var lhsEnumerator = lhs.GetEnumerator())
            using (var rhsEnumerator = rhs.GetEnumerator())
            {
                bool lhsMoved = lhsEnumerator.MoveNext();
                bool rhsMoved = rhsEnumerator.MoveNext();

                while (lhsMoved && rhsMoved)
                {
                    int compareResult = Comparer<T>.Default.Compare(lhsEnumerator.Current, rhsEnumerator.Current);
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

        public static IEnumerable<T> ToEmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }
    }
}
