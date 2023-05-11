using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class RefTest
    {

        [Fact]
        public void Test() 
        {
            AA aA = new AA() { Age = 11 };
            int a = 123;
            Test1(a,aA);


            Test2(out int b, out AA oy2cw2xmkvlz63sab5hazqd2rkwz7tbe7inslboyfpucai);

            void Test1(in int a,in AA b) 
            {
                b.Age = 12;
            }

            void Test2(out int a,out AA b) 
            {
                a = 1;
                b = new AA();
            }
        }

        private class AA
        {
            public int Age { get; set; }
        }
    }
}
