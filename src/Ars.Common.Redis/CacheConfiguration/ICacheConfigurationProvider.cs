using Ars.Common.Redis.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.CacheConfiguration
{
    public interface ICacheConfigurationProvider
    {
        void Configure(string cacheName, Action<IArsCacheOption> action);

        void ConfigureAll(Action<IArsCacheOption> action);

        List<ICacheConfiguration> cacheConfigurations { get; }  
    }
}
