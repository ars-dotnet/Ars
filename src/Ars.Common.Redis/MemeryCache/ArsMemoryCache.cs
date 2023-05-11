using Ars.Common.Core.IDependency;
using Ars.Common.Redis.Caching;
using Ars.Common.Redis.RedisCache.StringCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.MemeryCache
{
    internal class ArsMemoryCache : ArsCacheBaseAccessor, ICache
    {
        private readonly IMemoryCache _memeryCache;
        public ArsMemoryCache(ILogger<ArsMemoryCache> logger, IMemoryCache memoryCache) 
            : base(logger)
        {
            _memeryCache = memoryCache;
        }

        public override Task ClearAsync()
        {
            return Task.CompletedTask;
        }

        public override Task<ConditionalValue<TValue>> GetValueOrDefaultAsync<TValue>(string key)
        {
            throw new NotImplementedException();
        }

        public override Task<long> RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public override Task SetAsync<TValue>(string key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            throw new NotImplementedException();
        }
    }
}
