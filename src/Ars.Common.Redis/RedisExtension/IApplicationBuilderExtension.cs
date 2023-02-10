using Ars.Common.Redis.CacheConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisExtension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsRedis(this IApplicationBuilder applicationBuilder,
            Action<ICacheConfigurationProvider>? config = null)
        {
            var provider = applicationBuilder.ApplicationServices;
            if (null != config)
            {
                config.Invoke(provider.GetRequiredService<ICacheConfigurationProvider>());
            }

            var arsCacheOption = provider.GetRequiredService<IOptions<ArsCacheOption>>().Value;
            var csredis = new CSRedis.CSRedisClient($"{arsCacheOption.RedisConnection},defaultDatabase={arsCacheOption.DefaultDB},idleTimeout={arsCacheOption.IdleTimeout},poolsize={arsCacheOption.Poolsize}");
            RedisHelper.Initialization(csredis);

            return applicationBuilder;
        }
    }
}
