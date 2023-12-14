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
        public static IArsWebApplicationBuilder AddArsRedis(
            this IArsWebApplicationBuilder arsServiceBuilder,
            Action<ICacheConfigurationProvider>? provider = null)
        {
            var arsCacheOption = arsServiceBuilder.Configuration
                .GetSection(nameof(ArsCacheConfiguration))
                .Get<ArsCacheConfiguration>()
                ?? 
                throw new Exception("appsettings => ArsCacheConfiguration not be null");

            if (string.IsNullOrEmpty(arsCacheOption.RedisConnection))
                throw new ArgumentNullException(nameof(arsCacheOption.RedisConnection));

            arsServiceBuilder.ArsConfiguration.ArsRedisConfiguration ??= arsCacheOption;

            var service = arsServiceBuilder.Services;

            service.AddSingleton<IArsRedisConfiguration>(_ => arsCacheOption);

            ICacheConfigurationProvider cacheConfigurationProvider = new CacheConfigurationProvider();
            service.AddSingleton(_ => cacheConfigurationProvider);
            provider?.Invoke(cacheConfigurationProvider);

            var csredis = new CSRedis.CSRedisClient(
                $"{arsCacheOption.RedisConnection},defaultDatabase={arsCacheOption.DefaultDB}," +
                $"idleTimeout={arsCacheOption.IdleTimeout},poolsize={arsCacheOption.Poolsize}");

            RedisHelper.Initialization(csredis);

            return arsServiceBuilder;
        }
    }
}
