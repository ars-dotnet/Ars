using Ars.Commom.Tool.Serializer;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Redis.CacheProvider;
using Ars.Common.Redis.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache
{
    internal class ArsRedisCacheProvider : ArsCacheBaseProvider<ICache>, IArsCacheProvider,ISingletonDependency
    {
        private readonly IServiceProvider _serviceProvider;
        public ArsRedisCacheProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override ICache CreateImplementCache(string cachename)
        {
            var cache = _serviceProvider.GetRequiredService<ArsRedisCache>();
            cache.Name = cachename;
            return cache;
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
