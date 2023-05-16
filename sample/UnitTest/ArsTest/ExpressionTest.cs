using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class ExpressionTest
    {
        [Fact]
        public void TestUnaryExpression() 
        {
            Expression a = Expression.TypeAs(Expression.Constant(123,typeof(int)),typeof(Object));
            var t = a.ToString();
        }

        [Fact]
        public void ExpressionNew()
        {
            var a = New<T>.Instance();
        }
        
    }

    public class New<T> where T : new()
    {
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
        (
            Expression.New(typeof(T))
        ).Compile();
    }
}
