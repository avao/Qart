using System;
using System.Collections.Generic;
using System.Linq;

namespace Qart.Core.Enums
{
    public static class EnumUtils
    {
        public static IEnumerable<string> GetNames<T>()
            where T : struct, IComparable, IFormattable, IConvertible
            => Enum.GetNames(typeof(T));

        public static IEnumerable<T> GetValues<T>()
            where T : struct, IComparable, IFormattable, IConvertible
            => Enum.GetValues(typeof(T)).Cast<T>();
    }
}
