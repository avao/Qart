using Qart.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Qart.Core.Comparison
{
    public static class AssessmentHelper
    {
        public static void AssessMemberEquality<T, TM>(this T lhs, T rhs, Expression<Func<T, TM>> memberExpression, Action<T, T, TM, TM, MemberInfo> onDiffAction)
        {
            var compiled = memberExpression.Compile();
            var lhsMemberValue = compiled.Invoke(lhs);
            var rhsMemberValue = compiled.Invoke(rhs);
            if (!EqualityComparer<TM>.Default.Equals(lhsMemberValue, rhsMemberValue))
            {
                onDiffAction(lhs, rhs, lhsMemberValue, rhsMemberValue, memberExpression.GetMemberInfo());
            }
        }
    }
}
