using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExpressionDemo.TestType
{
    internal class Activetor
    {
        public void Test() 
        {
            var asms = AppDomain
                        .CurrentDomain
                        .GetAssemblies()
                        .Where(d => !d.IsDynamic);
            var handlerName = $"{nameof(Person<object>)}`1";
            var types = asms
                .SelectMany(d => d.GetTypes()
                .Where(t => null != t.BaseType && t.IsClass && t.BaseType.Name.Equals(handlerName)))
                .ToList();

            //MethodInfo m = typeof(TestPerson).GetMethod("myseak");
            //object[] ps = { 1 };
            //m.Invoke(p, ps);

            Console.Read();
        }
    }
}
