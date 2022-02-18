using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public abstract class ArsCacheBase : IArsCache
    {
        protected ArsCacheBase(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ArsCacheBase<TKey, TValue> : ArsCacheBase, IArsCache<TKey, TValue>
    {
        public ArsCacheBase(string name) : base(name)
        {

        }

        public Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            throw new NotImplementedException();
        }

        public Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }
    }
}
