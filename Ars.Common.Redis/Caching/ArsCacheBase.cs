using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ars.Commom.Tool.Extension;
using System.Diagnostics.CodeAnalysis;

namespace Ars.Common.Redis.Caching
{
    public abstract class ArsCacheBase : IArsCache,IDisposable
    {
        public ILogger Logger { get; set; }
        protected ArsCacheBase(ILogger logger)
        {
            Logger = logger;
        }

        public string Name { get; set; }

        public abstract Task ClearAsync();

        protected virtual string GetLocalizedRedisKey(string key) 
        {
            return key;
        }

        public virtual void Dispose()
        {
            
        }
    }

    public abstract class ArsCacheBase<TKey, TValue> : ArsCacheBase, IArsCache<TKey, TValue>, IArsCacheOption
    {
        protected ArsCacheBase(ILogger logger) : base(logger)
        {
            DefaultSlidingExpireTime = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(new Random().Next(5)));
        }

        protected readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public TimeSpan DefaultSlidingExpireTime { get; set; }
        public DateTimeOffset? DefaultAbsoluteExpireTime { get; set; }

        public virtual async Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            ConditionalValue<TValue> result = default;
            try
            {
                result = await GetValueOrDefaultAsync(key);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.ToString());
            }

            if (result.HasValue)
                return result.Value;

            await using (await SemaphoreSlim.LockAsync()) 
            {
                try
                {
                    result = await GetValueOrDefaultAsync(key);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.ToString());
                }

                if (result.HasValue)
                    return result.Value;

                var value = await factory(key);
                if (IsDefaultValue(value))
                    return value;

                try
                {
                    await SetAsync(key, value);
                }
                catch (Exception e) 
                {
                    Logger.LogError(e,e.ToString());
                }

                return value;
            }
        }

        public virtual bool IsDefaultValue(TValue value) 
        {
            return EqualityComparer<TValue>.Default.Equals(value, default);
        }

        public virtual async IAsyncEnumerable<TValue> GetAsync([NotNull]TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            foreach (var k in keys) 
            {
                yield return await GetAsync(k,factory);
            }
        }

        public abstract Task<ConditionalValue<TValue>> GetValueOrDefaultAsync(TKey key);

        public virtual async IAsyncEnumerable<ConditionalValue<TValue>> GetValueOrDefaultAsync([NotNull] TKey[] keys) 
        {
            foreach (var k in keys) 
            {
                yield return await GetValueOrDefaultAsync(k);
            }
        }

        public abstract Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        public virtual Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null) 
        {
            return Task.WhenAll(pairs.Select(r => SetAsync(r.Key,r.Value, slidingExpireTime, absoluteExpireTime)));
        }

        public abstract Task<long> RemoveAsync(TKey key);

        public virtual async IAsyncEnumerable<long> RemoveAsync([NotNull]TKey[] keys)
        {
            foreach (var k in keys) 
            {
                yield return await RemoveAsync(k);
            }
        }
    }
}
