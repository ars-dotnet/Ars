using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class WhileTrueTest
    {

        [Fact]
        public async Task Test()
        {
            await Task.WhenAll(A(),B());
            Console.Read();
        }

        private async Task A() 
        {
            Console.WriteLine("A-Start"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await C();
            Console.WriteLine("A-End" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private async Task B() 
        {
            Console.WriteLine("B-Start" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await C();
            Console.WriteLine("B-End" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private async Task C() 
        {
            await Task.Delay(1000);
        }
    }
}
