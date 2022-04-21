using Ars.Common.AutoFac;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Redis.CacheConfiguration;
using Ars.Common.Redis.Caching;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.CacheProvider
{
    public abstract class ArsCacheBaseProvider<TCache> : IArsCacheProvider<TCache>
        where TCache : class,IArsCacheOption
    {
        protected readonly ConcurrentDictionary<string, Lazy<TCache>> Caches;

        [Autowired]
        public ICacheConfigurationProvider cacheConfigurationProvider { get; set; }

        protected ArsCacheBaseProvider()
        {
            Caches = new ConcurrentDictionary<string, Lazy<TCache>>();
        }

        public virtual TCache GetArsCache(string cachename) 
        {
            var lazy = Caches.GetOrAdd(cachename,
                (key) => new Lazy<TCache>(
                    () => 
                    {
                        var cache = CreateImplementCache(key);

                        var config = cacheConfigurationProvider.cacheConfigurations.FirstOrDefault(r => string.IsNullOrEmpty(r.CacheName) || r.CacheName.Equals(key, StringComparison.OrdinalIgnoreCase));
                        if (null != config) 
                        {
                            config.Action(cache);
                        }

                        return cache;
                    }));

            return lazy.Value;
        }

        protected abstract void DisposeCaches();

        public virtual void Dispose() 
        {
            DisposeCaches();
            Caches.Clear();
        }

        protected abstract TCache CreateImplementCache(string cachename);
    }
}
