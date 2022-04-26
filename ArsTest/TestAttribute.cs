using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class TestAttribute
    {
        [Fact]
        public void Test() 
        {
            var type = typeof(AnimalDerivedA).GetTypeInfo();
            Assert.False(type.IsDefined(typeof(MyAttribute),false));
            Assert.True(type.IsDefined(typeof(MyAttribute), true));

            type = typeof(AnimalDerived).GetTypeInfo();
            int a = type.GetCustomAttributes<MyAttribute>(false).Count();
            int b = type.GetCustomAttributes<MyAttribute>(true).Count();
            Assert.True(a == 1);
            Assert.True(b > 1);
        }
    }

    [My("tom")]
    public class Animal 
    {
        public virtual string Name { get; set; }
    }

    public class AnimalDerivedA : Animal 
    {

    }

    [My("jerry")]
    public class AnimalDerived : Animal 
    {
        public override string Name { get => base.Name; set => base.Name = value; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true,Inherited = true)]
    public class MyAttribute : Attribute 
    {
        private string _name;
        public MyAttribute(string name)
        {
            _name = name;
        }
    }

    public class MyDerivedAttreibute : MyAttribute 
    {
        public MyDerivedAttreibute(string name) : base(name)
        {

        }
    }
}
