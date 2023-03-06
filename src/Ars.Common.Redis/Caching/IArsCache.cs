using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public interface IArsCache
    {
        string Name { get; }

        /// <summary>
        /// Clears all items in this cache.
        /// </summary>
        Task ClearAsync();
    }

    public interface IArsCache<TKey, TValue> : IArsCache
    {
        /// <summary>
        /// Gets an item from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        Task<TValue?> GetAsync(TKey key, Func<TKey, Task<TValue>>? factory = null);

        /// <summary>
        /// Gets items from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached items</returns>
        IAsyncEnumerable<TValue?> GetAsync(TKey[] keys, Func<TKey, Task<TValue>>? factory = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ConditionalValue<TValue>> GetValueOrDefaultAsync(TKey key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IAsyncEnumerable<ConditionalValue<TValue>> GetValueOrDefaultAsync(TKey[] keys);

        /// <summary>
        /// Saves/Overrides an item in the cache by a key.
        /// Use one of the expire times at most (<paramref name="slidingExpireTime"/> or <paramref name="absoluteExpireTime"/>).
        /// If none of them is specified, then
        /// <see cref="IArsCacheOption.DefaultAbsoluteExpireTime"/> will be used if it's not null. Othewise, <see cref="IArsCacheOption.DefaultSlidingExpireTime"/>
        /// will be used.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        /// <summary>
        /// Saves/Overrides items in the cache by the pairs.
        /// Use one of the expire times at most (<paramref name="slidingExpireTime"/> or <paramref name="absoluteExpireTime"/>).
        /// If none of them is specified, then
        /// <see cref="IArsCacheOption.DefaultAbsoluteExpireTime"/> will be used if it's not null. Othewise, <see cref="IArsCacheOption.DefaultSlidingExpireTime"/>
        /// will be used.
        /// </summary>
        /// <param name="pairs">Pairs</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        /// <summary>
        /// Removes a cache item by it's key (does nothing if given key does not exists in the cache).
        /// </summary>
        /// <param name="key">Key</param>
        Task<long> RemoveAsync(TKey key);

        /// <summary>
        /// Removes cache items by their keys.
        /// </summary>
        /// <param name="keys">Keys</param>
        IAsyncEnumerable<long> RemoveAsync(TKey[] keys);
    }
}
