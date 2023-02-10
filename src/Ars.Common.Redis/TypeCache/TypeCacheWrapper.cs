using Ars.Common.Redis.Caching;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.TypeCache
{
    public class TypeCacheWrapper<TKey, TValue> : ITypeCache<TKey, TValue>
    {
        private ICache _cache;
        public TypeCacheWrapper(ICache cache)
        {
            _cache = cache;
        }

        public ICache InnerCache => _cache;

        public TimeSpan DefaultSlidingExpireTime 
        {
            get => _cache.DefaultSlidingExpireTime;
            set => _cache.DefaultSlidingExpireTime = value;
        }

        public DateTimeOffset? DefaultAbsoluteExpireTime 
        {
            get => _cache.DefaultAbsoluteExpireTime;
            set => _cache.DefaultAbsoluteExpireTime = value;
        }

        public string Name => _cache.Name;

        public Task ClearAsync()
        {
            return _cache.ClearAsync();
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        private TValue CastOrDefault(object obj) 
        {
            if (null == obj)
                return default;

            if (obj.GetType() == typeof(JObject)) 
            {
                return ((JObject)obj).ToObject<TValue>();
            }

            return (TValue)obj;
        }

        public async Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            return CastOrDefault(await _cache.GetAsync(key.ToString(),async k => await factory?.Invoke(key)));
        }

        public async IAsyncEnumerable<TValue> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            foreach (var k in keys) 
            {
                yield return await GetAsync(k, factory);
            }
        }

        public async Task<ConditionalValue<TValue>> GetValueOrDefaultAsync(TKey key)
        {
            var value = await _cache.GetValueOrDefaultAsync(key.ToString());
            return new ConditionalValue<TValue>(value.HasValue, CastOrDefault(value.Value));
        }

        public async IAsyncEnumerable<ConditionalValue<TValue>> GetValueOrDefaultAsync(TKey[] keys)
        {
            foreach (var k in keys) 
            {
                yield return await GetValueOrDefaultAsync(k);
            }
        }

        public Task<long> RemoveAsync(TKey key)
        {
            return _cache.RemoveAsync(key.ToString());
        }

        public IAsyncEnumerable<long> RemoveAsync(TKey[] keys)
        {
            return _cache.RemoveAsync(keys.Select(r => r.ToString()).ToArray());
        }

        public Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            return _cache.SetAsync(key.ToString(), value, slidingExpireTime, absoluteExpireTime);
        }

        public async Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            foreach (var pair in pairs) 
            {
                await SetAsync(pair.Key, pair.Value, slidingExpireTime, absoluteExpireTime);
            }
        }
    }
}
