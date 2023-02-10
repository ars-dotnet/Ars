using Ars.Common.Redis.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache.HashCache
{
    public interface IArsHCache : IArsCache
    {
        
    }

    public interface IArsHCache<TKey, TField, TValue> : IArsHCache
    {
        Task<bool> HSetAsync(TKey key, TField field, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        Task<bool> HMSetAsync(TKey key,Dictionary<TField, TValue> keyValues, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        Task<TValue> HGetAsync<TValue>(TKey key, TField field);

        Task<Dictionary<TField, TValue>> HGetAllAsync<TValue>(TKey key);

        Task<TValue[]> HMGetAsync<TValue>(TKey key, params TField[] fields);

        Task<TValue> HGetAsync(TKey key, TField field);

        Task<Dictionary<TField, TValue>> HGetAllAsync(TKey key);

        Task<TValue[]> HMGetAsync(TKey key, params TField[] fields);

        Task<long> HDelAsync(string key, params string[] fields);
    }
}
