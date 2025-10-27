using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Errors;
using Ocelot.Logging;
using Ocelot.Provider.Polly;
using Ocelot.Requester;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot.Extension
{
    public static class IOcelotBuilderExtension
    {
        public static IOcelotBuilder AddArsPolly(this IOcelotBuilder builder) 
        {
            var errorMapping = new Dictionary<Type, Func<Exception, Error>>
            {
                {typeof(TaskCanceledException), e => new ArsRequestTimedOutError(e)},
                {typeof(TimeoutRejectedException), e => new ArsRequestTimedOutError(e)},
                //{typeof(BrokenCircuitException), e => new BrokenCircuitError(e)},
            };

            builder.Services.AddSingleton(_ => errorMapping);

            DelegatingHandler QosDelegatingHandlerDelegate(DownstreamRoute route, IHttpContextAccessor contextAccessor, IOcelotLoggerFactory logger)
            {
                return new ArsPollyCircuitBreakingDelegatingHandler(new ArsPollyQoSProvider(route, logger), logger);
            }

            builder.Services.AddSingleton(_ => (QosDelegatingHandlerDelegate)QosDelegatingHandlerDelegate);
            return builder;
        }
    }
}
