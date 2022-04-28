using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class TestAsyncLocal
    {
        private AsyncLocal<int> _local = new AsyncLocal<int>();

        /// <summary>
        /// 结论
        /// 外层调用异步包裹【值不变】
        /// 假异步 = 同步【值要变】
        /// 真异步【值不变】
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Test1()
        {
            _local.Value = 1;

            //异步包裹真异步，值不变
            await Task.Run(async () =>
            {
                await SetAsync();
            });
            Assert.Equal(1, _local.Value);

            //异步包裹同步，值不变
            await Task.Run(() =>
            {
                SetSync();
            });
            Assert.Equal(1, _local.Value);

            //异步包裹假异步，值不变
            await Task.Run(async () =>
            {
                await Set456();
            });
            Assert.Equal(1, _local.Value);

            //直接执行真异步，值不变
            var a = Thread.CurrentThread.ExecutionContext;
            int a1 = a.GetHashCode();
            await SetAsync().ConfigureAwait(true);
            a = Thread.CurrentThread.ExecutionContext;
            int a2 = a.GetHashCode();
            Assert.Equal(1, _local.Value);

            await SetAsync().ConfigureAwait(false);
            a = Thread.CurrentThread.ExecutionContext;
            int a3 = a.GetHashCode();
            Assert.Equal(1, _local.Value);

            //直接执行同步，值要变
            SetSync();
            Assert.Equal(234, _local.Value);

            //直接执行假异步，值要变
            await Set456();
            Assert.Equal(456, _local.Value);
        }

        private async Task SetAsync()
        {
            await Task.Yield();
            _local.Value = 123;
        }

        private void SetSync() 
        {
            _local.Value = 234;
        }

        private Task Set456()
        {
            _local.Value = 456;
            return Task.CompletedTask;
        }
    }
}
