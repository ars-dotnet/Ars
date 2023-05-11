using Ars.Commom.Tool.Serializer;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.IDependency;
using Ars.Common.Redis.CacheProvider;
using Ars.Common.Redis.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache.StringCache
{
    internal class ArsRedisCacheProvider : ArsCacheBaseProvider<ICache>, IArsCacheProvider, ISingletonDependency
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public ArsRedisCacheProvider(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override ICache CreateImplementCache(string cachename)
        {
            using var scope = _scopeFactory.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<ArsRedisCache>();
            cache.Name = cachename;
            return cache;
        }
    }
}
