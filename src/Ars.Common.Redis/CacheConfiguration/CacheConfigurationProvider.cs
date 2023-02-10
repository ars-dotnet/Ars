using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.IDependency;
using Ars.Common.Redis.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.CacheConfiguration
{
    public class CacheConfigurationProvider : ICacheConfigurationProvider,ISingletonDependency
    {
        public CacheConfigurationProvider()
        {
            cacheConfigurations = new List<ICacheConfiguration>(0);
        }

        public List<ICacheConfiguration> cacheConfigurations { get; protected set; }

        public void Configure(string cacheName,Action<IArsCacheOption> action)
        {
            cacheConfigurations.Add(new CacheConfiguration(cacheName, action));
        }

        public void ConfigureAll(Action<IArsCacheOption> action)
        {
            cacheConfigurations.RemoveAll(r => string.IsNullOrEmpty(r.CacheName));
            cacheConfigurations.Add(new CacheConfiguration(action));
        }
    }
}
