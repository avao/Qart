using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Qart.Core.Reflection
{
    public static class ExpressionExtensions
    {
        public static MemberInfo GetMemberInfo<T, TM>(this Expression<Func<T, TM>> expression)
        {
            return expression.Body.GetMemberInfo();
        }

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            return expression switch
            {
                MemberExpression memberExpression => memberExpression.Member,
                UnaryExpression unaryExpression => unaryExpression.Operand.GetMemberInfo(),
                _ => throw new Exception("Unsupported expression type")
            };
        }
    }
}
