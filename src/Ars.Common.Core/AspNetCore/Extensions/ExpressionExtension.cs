using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.Extensions
{
    public static class ExpressionExtension
    {
        public static Expression<Func<T, bool>> CombinExpression<T>(this Expression<Func<T, bool>> expression, Expression<Func<T, bool>> expression1)
        {
            if (null == expression)
                return expression1;
            if (null == expression1)
                return expression;

            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression1.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}
