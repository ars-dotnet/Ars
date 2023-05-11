using Ars.Common.Redis.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.RedisCache.HashCache
{
    public interface IHCache : IArsCacheOption, IArsHCache, IDisposable
    {

    }
}
