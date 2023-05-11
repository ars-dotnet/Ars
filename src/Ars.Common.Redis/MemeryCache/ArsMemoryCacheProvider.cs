using Ars.Common.Core.IDependency;
using Ars.Common.Redis.CacheProvider;
using Ars.Common.Redis.Caching;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.MemeryCache
{
    internal class ArsMemoryCacheProvider : ArsCacheBaseProvider<ICache>, IArsMemoryCacheProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ArsMemoryCacheProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override ICache CreateImplementCache(string cachename)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<ArsMemoryCache>();
            cache.Name = cachename;
            return cache;
        }
    }
}
