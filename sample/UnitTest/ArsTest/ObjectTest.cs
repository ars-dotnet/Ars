using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class ObjectTest
    {
        [Fact]
        public void Test1() 
        {
            Ars ars = new Ars();

            Assert.True(1 == ars.GetAge());
            
            ars = new Ars();

            Assert.True(1 == ars.GetAge());
        }
    }

    public class Ars
    {
        //private Ars()
        //{
        //    Age = 2;
        //}

        static Ars()
        {
            Age += 1;
        }

        ~Ars() 
        {

        }

        private static int Age { get; set; }

        public int GetAge() 
        {
            return Age;
        }
    }
}
