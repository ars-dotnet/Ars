using Ocelot.Logging;
using Ocelot.Provider.Polly;
using Polly.CircuitBreaker;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Polly.Wrap;
using Ars.Commom.Tool.Extension;

namespace Ars.Common.Ocelot
{
    public class ArsPollyCircuitBreakingDelegatingHandler : DelegatingHandler
    {
        private readonly ArsPollyQoSProvider _qoSProvider;
        private readonly IOcelotLogger _logger;
        public ArsPollyCircuitBreakingDelegatingHandler(
            ArsPollyQoSProvider qoSProvider,
            IOcelotLoggerFactory loggerFactory)
        {
            _qoSProvider = qoSProvider;
            _logger = loggerFactory.CreateLogger<ArsPollyCircuitBreakingDelegatingHandler>();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                Task<HttpResponseMessage> task;
                if (_qoSProvider.AsyncPolicies.HasValue())
                {
                    IAsyncPolicy<HttpResponseMessage>? policyWrap = null;
                    foreach (var policy in _qoSProvider.AsyncPolicies!) 
                    {
                        policyWrap = 
                            null == policyWrap 
                            ? policy 
                            : policyWrap.WrapAsync(policy);
                    }

                    foreach (var policy in _qoSProvider.CircuitBreaker.Policies) 
                    {
                        policyWrap = policyWrap!.WrapAsync(policy);
                    }

                    task = policyWrap!.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
                }
                else 
                {
                    task = Policy
                       .WrapAsync(_qoSProvider.CircuitBreaker.Policies)
                       .ExecuteAsync(() => base.SendAsync(request, cancellationToken));
                }

                return task;
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError("Reached to allowed number of exceptions. Circuit is open", ex);
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Error in CircuitBreakingDelegatingHandler.SendAync", ex);
                throw;
            }
        }
    }
}
