using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public interface IArsCacheBase
    {
        string Name { get; }

        /// <summary>
        /// Clears all items in this cache.
        /// </summary>
        Task ClearAsync();
    }

    public interface IArsCache : IArsCacheBase
    {
        /// <summary>
        /// Gets an item from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        Task<TValue?> GetAsync<TValue>(string key, Func<string, Task<TValue>>? factory = null);

        /// <summary>
        /// Gets items from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached items</returns>
        IAsyncEnumerable<TValue?> GetAsync<TValue>(string[] keys, Func<string, Task<TValue>>? factory = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ConditionalValue<TValue>> GetValueOrDefaultAsync<TValue>(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IAsyncEnumerable<ConditionalValue<TValue>> GetValueOrDefaultAsync<TValue>(string[] keys);

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
        Task SetAsync<TValue>(string key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

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
        Task SetAsync<TValue>(KeyValuePair<string, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        /// <summary>
        /// Removes a cache item by it's key (does nothing if given key does not exists in the cache).
        /// </summary>
        /// <param name="key">Key</param>
        Task<long> RemoveAsync(string key);

        /// <summary>
        /// Removes cache items by their keys.
        /// </summary>
        /// <param name="keys">Keys</param>
        IAsyncEnumerable<long> RemoveAsync(string[] keys);
    }
}
