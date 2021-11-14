using System;
using System.Collections.Generic;

namespace Qart.Core.Comparison
{
    public class LambdaBasedConverterComparer<T, TValue> : IComparer<T>
    {
        private readonly Func<T, TValue> _converterFunc;
        private readonly IComparer<TValue> _comparer;

        public LambdaBasedConverterComparer(Func<T, TValue> converterFunc)
            : this(converterFunc, Comparer<TValue>.Default)
        {
        }

        public LambdaBasedConverterComparer(Func<T, TValue> converterFunc, IComparer<TValue> comparer)
        {
            _converterFunc = converterFunc;
            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer.Compare(_converterFunc.Invoke(x), _converterFunc.Invoke(y));
        }
    }
}
