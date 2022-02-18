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
        Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory);

        /// <summary>
        /// Gets items from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached items</returns>
        Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory);

        /// <summary>
        /// Gets an item from the cache.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Cache item</param>
        /// <returns>Result to indicate cache hit</returns>
        bool TryGetValue(TKey key, out TValue value);
    }
}
