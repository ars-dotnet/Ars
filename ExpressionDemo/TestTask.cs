using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo
{
    internal class TestTask
    {
        public async Task Get1() 
        {
            return ;
        }

        public async Task Get2() 
        {
            await Get1();
            return;
        }

        public Task Get3() 
        {
            return Task.CompletedTask;
        }
    }
}
