using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Qart.Core.Comparison
{
    public static class PairExtensions
    {
        public static void AssessEquality<T, TM>(this (T, T) pair, Expression<Func<T, TM>> memberExpression, Action<T, T, TM, TM, MemberInfo> onDiffAction)
        {
            pair.Item1.AssessMemberEquality(pair.Item2, memberExpression, onDiffAction);
        }
    }
}
