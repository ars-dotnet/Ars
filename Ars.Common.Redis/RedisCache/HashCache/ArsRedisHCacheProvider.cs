using Ars.Common.AutoFac.IDependency;
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
        private readonly IServiceProvider _serviceProvider;
        public ArsRedisHCacheProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override IHCache CreateImplementCache(string cachename)
        {
            var hcache = _serviceProvider.GetRequiredService<ArsRedisHCache>();
            hcache.Name = cachename;

            return hcache;
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches) 
            {
                cache.Value.Value.Dispose();
            }
        }
    }
}
