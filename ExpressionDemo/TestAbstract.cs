using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo
{
    public interface ITestAbstract 
    {
        public static string name = "123";
    }

    public class TestAbstractB : ITestAbstract 
    {
        public TestAbstractB()
        {

        }
    }

    public abstract class TestAbstract
    {
        public TestAbstract(int a)
        {
            num = a;
        }

        internal int num;

        internal abstract void Get();

        internal virtual string Hello() 
        {
            return "Hello";
        }

        internal string Say() 
        {
            return Hello();
        }
        internal virtual string Cry()
        {
            return Hello();
        }
    }

    public class TestAbstractAA : TestAbstract
    {
        public TestAbstractAA(int num) : base(num) 
        {
            num = 2;
        }

        internal override void Get()
        {

        }
    }

    public class TestAbstractA : TestAbstract
    {
        public TestAbstractA(int i) : base(i)
        {
            num = 1;
        }
        
        internal override void Get()
        {
            this.Say();
            base.Say();
            Hello();
        }

        internal override string Hello()
        {
            return "Hello World";
        }

        internal new string Say() 
        {
            return "Say";
        }
    }
}
