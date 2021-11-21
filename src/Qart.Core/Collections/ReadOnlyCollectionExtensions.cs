using System;
using System.Collections.Generic;

namespace Qart.Core.Collections
{
    public static class ReadOnlyCollectionExtensions
    {
        public static IReadOnlyCollection<T> ToEmptyIfNull<T>(this IReadOnlyCollection<T> items)
            => items ?? Array.Empty<T>();
    }
}
