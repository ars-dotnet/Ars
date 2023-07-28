using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class InheritTest
    {
        [Fact]
        public void Test1() 
        {
            Animal a = new Dog();
            Assert.True(a.Name.Equals("Dog"));

            Animal.Eat();
            Dog.Eat();
        }

        class Animal 
        {
            public string Name { get;set;}

            public static void Eat() 
            {

            }

            public void Run() 
            {
                GetType();
            }
        }

        class Dog : Animal 
        {
            public Dog()
            {
                this.Name = "Dog";
            }
        }
    }
}
