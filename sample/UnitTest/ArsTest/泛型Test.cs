using Ars.Common.Tool.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class 泛型Test
    {
        [Fact]
        public void Test1()
        {
            Assert.False(typeof(IAnimal<>).IsAssignableFrom(typeof(Dog)));
            Assert.True(typeof(IAnimal<int>).IsAssignableFrom(typeof(Dog)));

            Assert.True(typeof(IAnimal<>).IsAssignableGenericFrom(typeof(Dog)));
            Assert.False(typeof(IAnimal<int>).IsAssignableGenericFrom(typeof(Dog)));
        }

        [Fact]
        public void Test2() 
        {
            Assert.False(typeof(IAnimal<>).IsAssignableFrom(typeof(Dog<>)));
            Assert.True(typeof(IAnimal<int>).IsAssignableFrom(typeof(Dog<>)));

            Assert.True(typeof(IAnimal<>).IsAssignableGenericFrom(typeof(Dog<>)));
            Assert.False(typeof(IAnimal<int>).IsAssignableGenericFrom(typeof(Dog<>)));
        }
    }

    public interface IAnimal { }

    public interface IAnimal<T> : IAnimal
    {
        T Type { get; set; }
    }

    public class Dog : IAnimal<int>
    {
        public int Type { get; set; } = 123;
    }

    public class Dog<T> : Dog
    {
        public T Name { get; set; }
    }
}
