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
    public abstract class ArsCacheBase : IArsCacheBase, IArsCacheOption, IDisposable
    {
        protected ILogger Logger;
        protected ArsCacheBase(ILogger logger)
        {
            Logger = logger;
        }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        public DateTimeOffset? DefaultAbsoluteExpireTime { get; set; }


        public string Name { get; set; }

        public abstract Task ClearAsync();

        protected virtual string GetLocalizedCacheKey(string key) 
        {
            return key;
        }

        public virtual void Dispose()
        {
            
        }
    }

    public abstract class ArsCacheBaseAccessor: ArsCacheBase , IArsCache
    {
        protected ArsCacheBaseAccessor(ILogger logger) : base(logger)
        {
            DefaultSlidingExpireTime = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(new Random().Next(5)));
        }

        /// <summary>
        /// 锁同一个ArsCacheBase的子类实例
        /// </summary>
        protected readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        
        public virtual bool IsDefaultValue<TValue>(TValue value)
        {
            return EqualityComparer<TValue>.Default.Equals(value, default);
        }

        public virtual async Task<TValue?> GetAsync<TValue>(string key, Func<string, Task<TValue>>? factory = null)
        {
            ConditionalValue<TValue> result = default;
            try
            {
                result = await GetValueOrDefaultAsync<TValue>(key);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.ToString());
            }

            if (result.HasValue)
                return result.Value!;

            var res = await SemaphoreSlim.LockAsync(async () =>
            {
                try
                {
                    result = await GetValueOrDefaultAsync<TValue>(key);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.ToString());
                }

                if (result.HasValue)
                    return result.Value;

                if (null == factory)
                    return default;
                var value = await factory(key);
                if (IsDefaultValue(value))
                    return value;

                try
                {
                    await SetAsync(key, value);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.ToString());
                }

                return value;
            });
            return res!;
        }

        public virtual async IAsyncEnumerable<TValue?> GetAsync<TValue>([NotNull]string[] keys, Func<string, Task<TValue>>? factory = null)
        {
            foreach (var k in keys) 
            {
                yield return await GetAsync(k,factory);
            }
        }

        public abstract Task<ConditionalValue<TValue>> GetValueOrDefaultAsync<TValue>(string key);

        public virtual async IAsyncEnumerable<ConditionalValue<TValue>> GetValueOrDefaultAsync<TValue>([NotNull] string[] keys) 
        {
            foreach (var k in keys) 
            {
                yield return await GetValueOrDefaultAsync<TValue>(k);
            }
        }

        public abstract Task SetAsync<TValue>(string key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        public virtual Task SetAsync<TValue>(KeyValuePair<string, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null) 
        {
            return Task.WhenAll(pairs.Select(r => SetAsync(r.Key,r.Value, slidingExpireTime, absoluteExpireTime)));
        }

        public abstract Task<long> RemoveAsync(string key);

        public virtual async IAsyncEnumerable<long> RemoveAsync([NotNull]string[] keys)
        {
            foreach (var k in keys) 
            {
                yield return await RemoveAsync(k);
            }
        }
    }
}
