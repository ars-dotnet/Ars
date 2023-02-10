using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class OperatorTest
    {
        [Fact]
        public void Test() 
        {
            int a = 2;
            int b = a << 0;
            int c = a << 2;

            int d = c >> 1;

            Assert.True(b == a);
            Assert.True(c == 8);
            Assert.True(d == 4);
        }
    }
}
