using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class InterlockedTest
    {
        [Fact]
        public void Test1() 
        {
            var a = Interlocked.Increment(ref A.count);
            var b = Interlocked.Increment(ref A.count);

            Assert.True(1 == a);
            Assert.True(2 == b);
        }

        class A 
        {
            public static int count = 0;
        }
    }
}
