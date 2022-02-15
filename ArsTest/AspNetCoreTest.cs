using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class AspNetCoreTest
    {
        [Fact]
        public void CreateHost() 
        {
            int[] array = { 1, 2, 3, 4, 5, 6 };
            Array.ForEach(array, r => 
            {
                r++;
            });
            
            
        }
    }
}
