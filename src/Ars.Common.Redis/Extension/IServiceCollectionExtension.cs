using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Redis.CacheConfiguration;
using Ars.Common.Redis.RedisCache.StringCache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisExtension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsRedis(
            this IArsServiceBuilder arsServiceBuilder, 
            Action<ICacheConfigurationProvider>? provider = null) 
        {
            var arsCacheOption = arsServiceBuilder.Services.Provider.GetRequiredService<IConfiguration>()
                .GetSection(nameof(ArsCacheConfiguration))
                .Get<ArsCacheConfiguration>() 
                ?? throw new Exception("appsettings => ArsCacheConfiguration not be null");
            var arsconfig = arsServiceBuilder.Services.Provider.GetRequiredService<IArsConfiguration>();
            arsconfig.ArsRedisConfiguration ??= arsCacheOption;

            var service = arsServiceBuilder.Services.ServiceCollection;
            service.AddSingleton<IArsRedisConfiguration>(arsCacheOption);
            service.AddSingleton<ICacheConfigurationProvider>(new CacheConfigurationProvider());
            if(null != provider)
                provider(arsServiceBuilder.Services.Provider.GetRequiredService<ICacheConfigurationProvider>());

            if (string.IsNullOrEmpty(arsCacheOption.RedisConnection))
                throw new ArgumentNullException(nameof(arsCacheOption.RedisConnection));

            //service.AddSingleton<IArsCacheProvider, ArsRedisCacheProvider>();

            var csredis = new CSRedis.CSRedisClient($"{arsCacheOption.RedisConnection},defaultDatabase={arsCacheOption.DefaultDB},idleTimeout={arsCacheOption.IdleTimeout},poolsize={arsCacheOption.Poolsize}");
            RedisHelper.Initialization(csredis);

            return arsServiceBuilder;
        }
    }
}
