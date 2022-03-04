using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public delegate void Smoke(int a);
    public class TestEvent 
    {
        public event Smoke @event;

        [Fact]
        public void testEvent() 
        {
            Atest atest = new (this);
            atest.Add();
            @event?.Invoke(123);
            atest.Action();
            Console.ReadLine();
        }
    }

    public class Atest 
    {
        private readonly TestEvent testEvent;
        private Smoke smoke;
        public Atest(TestEvent @event)
        {
            this.testEvent = @event;
        }

        internal void Add() 
        {
            testEvent.@event += SmokeA;
            smoke = new Smoke(SmokeA);
            smoke = null;
            smoke += SmokeB;
        }

        internal void Action() 
        {
            smoke(123);
        }

        private void SmokeA(int  a) 
        {
            Console.WriteLine("SmokeA" + a);
        }

        private void SmokeB(int a)
        {
            Console.WriteLine("SmokeB" + a);
        }
    }
}
