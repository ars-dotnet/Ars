using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionDemo
{
    public static class MyExpressionVisitorExtension
    {
        private static Lazy<MyExpressionVisitor> myExpression;
        static MyExpressionVisitorExtension() 
        {
            myExpression = new Lazy<MyExpressionVisitor>();
        }

        public static Expression Modify(this Expression expression) 
        {
            return myExpression.Value.Modify(expression);
        }
    }

    public class MyExpressionVisitor : ExpressionVisitor
    {
        public Expression Modify(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Add)
            {
                Expression left = this.Visit(node.Left);
                Expression right = this.Visit(node.Right);
                return Expression.Subtract(left, right);
            }

            return base.VisitBinary(node);
        }
    }
}
