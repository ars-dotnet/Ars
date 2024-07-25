using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class ChangeTokenTest
    {
        [Fact]
        public void Test1() 
        {
            int i = 0;
            TestCancellationChangeToken testCancellationChangeToken = new TestCancellationChangeToken();

            ChangeToken.OnChange(
                () => testCancellationChangeToken.CreatChanageToken(),
                () => i++);

            for (int j = 0; j < 5; j++)
            {
                testCancellationChangeToken.CancelToken();
            }

            Assert.True(5 == i);
        }

        [Fact]
        public void Test2()  
        {
            int a = 123;

            int b = Interlocked.Exchange(ref a, 234);

            Assert.True(234 == a);
            Assert.True(123 == b);
        }
    }

    public class TestCancellationChangeToken
    {
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// 获取CancellationChangeToken实例方法
        /// </summary>
        public CancellationChangeToken CreatChanageToken()
        {
            var x = Interlocked.Exchange(ref tokenSource, new CancellationTokenSource());

            return new CancellationChangeToken(tokenSource.Token);
        }

        /// <summary>
        /// 取消CancellationTokenSource
        /// </summary>
        public void CancelToken()
        {
            tokenSource.Cancel();
        }
    }
}
