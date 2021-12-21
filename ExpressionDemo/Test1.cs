using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionDemo
{
    public class Test1<T> where T : class
    {
        public T data { get; }

        public int Get()
        {
            return 0;
        }
    }

    public class TestQuerable<T> : IQueryable<T>
    {
        public TestQuerable()
        {
            Provider = new MyQueryProvider();
            Expression = Expression.Constant(this);
        }

        public TestQuerable(IMyQueryProvider myQueryProvider, Expression expression)
        {
            if (null == myQueryProvider)
                throw new ArgumentNullException(nameof(myQueryProvider));
            if (null == expression)
                throw new ArgumentNullException(nameof(expression));
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentException($"{typeof(IQueryable<T>)}not IsAssignableFrom({expression.Type})");
            Provider = myQueryProvider;
            Expression = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }
    }

    public interface IMyQueryProvider : IQueryProvider 
    {

    }

    public class MyQueryProvider : IMyQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            Type[] paramTypes = expression.Type.GetGenericArguments();
            Type paramType = paramTypes.First();
            return Activator.CreateInstance(typeof(IQueryable<>).MakeGenericType(paramType),new object[] { this, expression }) as IQueryable;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestQuerable<TElement>(this,expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
