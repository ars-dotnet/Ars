using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public abstract class CacheBase : ArsCacheBase<string, object>, ICache
    {
        public CacheBase(string name) : base(name)
        {

        }

        public TimeSpan DefaultSlidingExpireTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTimeOffset? DefaultAbsoluteExpireTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name => throw new NotImplementedException();

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetAsync(string key, Func<string, Task<object>> factory)
        {
            throw new NotImplementedException();
        }

        public Task<object[]> GetAsync(string[] keys, Func<string, Task<object>> factory)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }
    }
}
