using Ars.Common.Redis.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache.HashCache
{
    public interface IArsHCache : IArsCacheBase
    {
        Task<bool> HSetAsync(string key, string field, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        Task<bool> HMSetAsync(string key,Dictionary<string, object> keyValues, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        Task<TValue> HGetAsync<TValue>(string key, string field);

        Task<Dictionary<string, TValue>> HGetAllAsync<TValue>(string key);

        Task<TValue[]> HMGetAsync<TValue>(string key, params string[] fields);

        Task<long> HDelAsync(string key, params string[] fields);

        Task<string[]> HKeysAsync(string key);
    }
}
