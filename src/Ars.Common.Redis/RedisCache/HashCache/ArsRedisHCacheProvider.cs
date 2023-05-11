using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.IDependency;
using Ars.Common.Redis.CacheProvider;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache.HashCache
{
    internal class ArsRedisHCacheProvider : ArsCacheBaseProvider<IHCache>, IArsHCacheProvider,ISingletonDependency
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public ArsRedisHCacheProvider(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override IHCache CreateImplementCache(string cachename)
        {
            using var scopee = _scopeFactory.CreateScope();
            var hcache = scopee.ServiceProvider.GetRequiredService<ArsRedisHCache>();
            hcache.Name = cachename;

            return hcache;
        }
    }
}
