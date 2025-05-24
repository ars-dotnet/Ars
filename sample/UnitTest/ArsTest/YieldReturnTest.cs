using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class YieldReturnTest
    {
        [Fact]
        public async void Test1() 
        {
            await foreach (var t in Get()) 
            {

            }
        }

        public async IAsyncEnumerable<int> Get()
        {
            List<int> i = new List<int> { 1, 2, 3 };

            foreach(var s in i.Where(r => r < 1))
            {
                await Task.Yield();

                yield return s;
            }
        }
    }
}
