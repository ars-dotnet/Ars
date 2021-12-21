using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo.TestType
{
    internal class Person
    {
        internal string name { get; set; }

        internal int age { get; set; }

        internal virtual void speak() 
        {
            Console.WriteLine($"my name is {name},and i am {age} years old.");
        }
    }

    internal class Person<T> : Person
    {
        internal override void speak()
        {
            base.speak();
            Console.WriteLine($"this is my type {typeof(T)}");
        }

        internal virtual void myseak(T t) 
        {
            Console.WriteLine($"this is param {t}");
        }
    }

    internal class TestPerson1 : Person<string>
    {
        public void Test1() { }

        private void Test() { }

        protected void Test2() { }

        internal void Test3() { }

        internal override void speak()
        {
            base.speak();
        }

        internal override void myseak(string t)
        {
            base.myseak(t);
        }
    }

    internal class TestPerson : Person<int>
    {
        public void Test1() { }

        private void Test() { }

        protected void Test2() { }

        internal override void speak()
        {
            base.speak();
        }

        internal override void myseak(int t)
        {
            base.myseak(t);
        }
    }

}
