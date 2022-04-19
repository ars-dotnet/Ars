using Ars.Commom.Core;
using Ars.Common.Redis.CacheConfiguration;
using Ars.Common.Redis.RedisCache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsRedis(this IArsServiceBuilder arsServiceBuilder) 
        {
            arsServiceBuilder.Services.ServiceCollection.AddSingleton<IArsCacheProvider, ArsRedisCacheProvider>();
            return arsServiceBuilder;
        }

        public static IApplicationBuilder UseArsRedis(this IApplicationBuilder applicationBuilder, 
            Action<ArsCacheOption> action, 
            Action<ICacheConfigurationProvider> config = null)
        {
            if (null != config)
            {
                var provider = applicationBuilder.ApplicationServices;
                config.Invoke(provider.GetRequiredService<ICacheConfigurationProvider>());
            }

            ArsCacheOption arsCacheOption = new ArsCacheOption();
            action?.Invoke(arsCacheOption);
            if (string.IsNullOrEmpty(arsCacheOption.RedisConnection))
                throw new ArgumentNullException(nameof(arsCacheOption.RedisConnection));

            var csredis = new CSRedis.CSRedisClient($"{arsCacheOption.RedisConnection},defaultDatabase={arsCacheOption.DefaultDB},idleTimeout={arsCacheOption.IdleTimeout},poolsize={arsCacheOption.Poolsize}");
            RedisHelper.Initialization(csredis);

            return applicationBuilder;
        }
    }
}
