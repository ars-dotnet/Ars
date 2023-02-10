using Ars.Common.Redis.Caching;
using Ars.Common.Redis.RedisCache.HashCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis
{
    public interface IArsCacheProvider<TCache> : IDisposable
        where TCache : class
    {
        TCache GetArsCache(string cachename);
    }

    public interface IArsCacheProvider : IArsCacheProvider<ICache> 
    {

    }

    public interface IArsHCacheProvider : IArsCacheProvider<IHCache> 
    {

    }
}
