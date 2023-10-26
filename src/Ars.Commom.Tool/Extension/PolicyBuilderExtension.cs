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
        public static IAsyncPolicy<T> AddArsHttpClientPolicy<T>(this PolicyBuilder<T> builder)
            where T : HttpResponseMessage
        {
            var breakerPolicy = builder
                .Or<TimeoutRejectedException>()
                .Or<TimeoutException>()
                .CircuitBreakerAsync(30, TimeSpan.FromSeconds(5));

            var timeOutPolicy = Policy.TimeoutAsync<T>(10);

            var retryPolicy = builder.WaitAndRetryAsync(
                new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });

            return breakerPolicy.WrapAsync(timeOutPolicy).WrapAsync(retryPolicy);
        }
    }
}
