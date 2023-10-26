using Ocelot.Configuration;
using Ocelot.Logging;
using Ocelot.Provider.Polly;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot
{
    public class ArsPollyQoSProvider
    {
        private readonly AsyncTimeoutPolicy _timeoutPolicy;
        private readonly IOcelotLogger _logger;
        private static readonly Func<HttpResponseMessage, bool> _transientHttpStatusCodePredicate = 
            (HttpResponseMessage response) => 
                response.StatusCode >= HttpStatusCode.InternalServerError 
             || response.StatusCode == HttpStatusCode.RequestTimeout;

        public ArsPollyQoSProvider(DownstreamRoute route, IOcelotLoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PollyQoSProvider>();

            Enum.TryParse(route.QosOptions.TimeoutStrategy, out TimeoutStrategy strategy);

            _timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(route.QosOptions.TimeoutValue), strategy);

            if (route.QosOptions.ExceptionsAllowedBeforeBreaking > 0)
            {
                AsyncPolicies ??= new IAsyncPolicy<HttpResponseMessage>[2];

                AsyncPolicies[0] = Policy<HttpResponseMessage>
                    .Handle<BrokenCircuitException>()
                    .Or<HttpRequestException>()
                    .FallbackAsync(_ => FallbackAsync());

                AsyncPolicies[1] = Policy<HttpResponseMessage>
                    .HandleResult(_transientHttpStatusCodePredicate)
                    .Or<HttpRequestException>()
                    .Or<TimeoutRejectedException>()
                    .Or<TimeoutException>()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: route.QosOptions.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: TimeSpan.FromMilliseconds(route.QosOptions.DurationOfBreak),
                        onBreak: (ex, breakDelay) =>
                        {
                            _logger.LogError(
                                ".Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!", ex.Exception);
                        },
                        onReset: () =>
                        {
                            _logger.LogDebug(".Breaker logging: Call ok! Closed the circuit again.");
                        },
                        onHalfOpen: () =>
                        {
                            _logger.LogDebug(".Breaker logging: Half-open; next call is a trial.");
                        }
                    );
            }

            CircuitBreaker = new CircuitBreaker(_timeoutPolicy);
        }

        public CircuitBreaker CircuitBreaker { get; }

        public IAsyncPolicy<HttpResponseMessage>[]? AsyncPolicies { get; }

        private Task<HttpResponseMessage> FallbackAsync()
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.GatewayTimeout));
        }
    }
}
