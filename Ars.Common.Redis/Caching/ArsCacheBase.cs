using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public abstract class ArsCacheBase : IArsCache,IDisposable
    {
        protected ArsCacheBase(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public abstract Task ClearAsync();

        public virtual void Dispose()
        {

        }
    }

    public abstract class ArsCacheBase<TKey, TValue> : ArsCacheBase, IArsCache<TKey, TValue>, IArsCacheOption
    {
        protected ArsCacheBase(string name) : base(name)
        {
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);
        }

        protected readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public TimeSpan DefaultSlidingExpireTime { get; set; }
        public DateTimeOffset? DefaultAbsoluteExpireTime { get; set; }

        public virtual Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            throw new NotImplementedException();
        }

        public abstract bool TryGetValue(TKey key, out TValue value);

        public virtual Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task RemoveAsync(TKey key)
        {
            throw new NotImplementedException();
        }

        public virtual Task RemoveAsync(TKey[] keys)
        {
            throw new NotImplementedException();
        }
    }
}
