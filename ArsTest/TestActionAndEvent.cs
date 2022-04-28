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
        public Smoke smoke;
        [Fact]
        public void testEvent()
        {
            Atest atest = new(this);
            atest.Add();
            @event?.Invoke(123);
            atest.Action();
            smoke.Invoke(1234);
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
            foreach (var i in new[] { 0,1,2,3,4,5 })
            {
                testEvent.@event += _ => SmokeA(i);
            }
            
            smoke = new Smoke(SmokeA);
            smoke = null;
            smoke += SmokeB;

            testEvent.smoke = null;
            testEvent.smoke += SmokeB;
        }

        internal void Action()
        {
            smoke(123);
        }

        private void SmokeA(int a)
        {
            Console.WriteLine("SmokeA" + a);
        }

        private void SmokeB(int a)
        {
            Console.WriteLine("SmokeB" + a);
        }
    }
}
