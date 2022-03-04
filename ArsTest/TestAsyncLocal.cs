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
        private static AsyncLocal<int> _local = new AsyncLocal<int>();
        private static AsyncLocal<int> _local1 = new AsyncLocal<int>();

        [Fact]
        public async Task TestSynchronizationContext() 
        {
            var context = SynchronizationContext.Current;
            int c1 = context.GetHashCode();
            _local.Value = 1;
            await Set().ConfigureAwait(true);
            var context1 = SynchronizationContext.Current;
            int c2 = context1.GetHashCode();
            await Set().ConfigureAwait(false);
            var context2 = SynchronizationContext.Current;
            int c3 = context2.GetHashCode();
        }

        [Fact]
        public async Task Test1()
        {
            _local.Value = 1;
            _local1.Value = 2;
            var a = Thread.CurrentThread.ExecutionContext;
            int a1 = a.GetHashCode();
            await Set().ConfigureAwait(true);
            a = Thread.CurrentThread.ExecutionContext;
            int a2 = a.GetHashCode();
            Assert.Equal(1, _local.Value);

            await Set().ConfigureAwait(false);
            a = Thread.CurrentThread.ExecutionContext;
            int a3 = a.GetHashCode();
            Assert.Equal(1, _local.Value);

            await Task.Run(() =>
            {
                SetSync();
            });
            Assert.Equal(1, _local.Value);

            SetSync();
            Assert.Equal(234, _local.Value);

            await Task.Run(() =>
            {
                Set456();
            });
            Assert.Equal(234, _local.Value);

            await Set456();
            Assert.Equal(456, _local.Value);
        }

        private async Task Set()
        {
            _local.Value = 123;
        }

        private void SetSync() 
        {
            _local.Value = 234;
        }

        private Task Set456()
        {
            _local.Value = 456;
            return Task.FromResult(1);
        }
    }
}
