using NPOI.SS.Formula.Functions;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class PolicyBuilderExtension
    {
        /// <summary>
        ///  降级 熔断 超时 重试 
        ///  超时时间10s
        ///  HttpRequestException、TimeoutRejectedException、TimeoutException、
        ///  HttpStatusCode >= 500 || HttpStatusCode == 408[请求超时] 都会触发熔断异常请求计数
        ///  熔断异常请求达到30个时，服务熔断5s
        ///  第一次请求失败后，此后有2次重试请求，分别为第1s和第2s
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IAsyncPolicy<T> AddArsHttpClientPolicy<T>(this PolicyBuilder<T> builder)
            where T : HttpResponseMessage
        {
            var fallbackPlicy = Policy<T>.Handle<BrokenCircuitException>().FallbackAsync(_ => 
            {
                return Task.FromResult((T)new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            });

            var breakerPolicy = builder
                .Or<TimeoutRejectedException>()
                .Or<TimeoutException>()
                .CircuitBreakerAsync(30, TimeSpan.FromSeconds(5));

            var timeOutPolicy = Policy.TimeoutAsync<T>(10);

            var retryPolicy = builder.WaitAndRetryAsync(
                new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });

            return fallbackPlicy.WrapAsync(breakerPolicy).WrapAsync(timeOutPolicy).WrapAsync(retryPolicy);
        }
    }
}
