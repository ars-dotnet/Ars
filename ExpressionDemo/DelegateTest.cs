using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo
{
    public class DelegateTest
    {
        public delegate int HelloWorldDelete();
        private event HelloWorldDelete @event;
        public void Add(HelloWorldDelete helloWorld) 
        {
            @event += helloWorld;
        }

        public int Action() 
        {
            return @event?.Invoke() ?? 0;
        }

        private int test1() 
        {
            Console.WriteLine("test1");
            return 0;
        }

        private int test2()
        {
            Console.WriteLine("test2");
            return 1;
        }
    }
}
