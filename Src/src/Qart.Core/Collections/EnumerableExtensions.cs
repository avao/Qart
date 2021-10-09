using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Collections
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEmptyIfNull<T>(this IEnumerable<T> items) 
            => items ?? Enumerable.Empty<T>();
    }
}
