using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class ManualResetEventTest
    {
        [Fact]
        public async Task Test()
        {
            //false无信号
            ManualResetEvent eventObj = new ManualResetEvent(false);

            int i = 0;

            Task.Run(() =>
            {
                eventObj.WaitOne();

                i = 100;
            });

            await Task.Delay(1000 * 2);

            //设置有信号，waitone才释放
            //设置一次，此后的waitone都会被释放
            //若想此后waitone被阻塞，则调用ReSet()
            eventObj.Set();

            await Task.Delay(1000 * 1);

            eventObj.WaitOne();

            i = 200;

            Assert.True(200 == i);
        }

        [Fact]
        public async Task Test1()
        {
            //false无信号
            AutoResetEvent eventObj = new AutoResetEvent(false);

            int i = 0;

            Task.Run(() =>
            {
                eventObj.WaitOne();

                i = 100;
            });

            await Task.Delay(1000 * 2);

            //设置有信号，waitone才释放
            eventObj.Set();

            await Task.Delay(1000 * 1);

            Task.Run(() =>
            {
                eventObj.WaitOne();

                i = 200;
            });

            //设置有信号，waitone才释放
            eventObj.Set();

            await Task.Delay(100 * 1);

            Assert.True(200 == i);
        }
    }
}
