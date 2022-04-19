using Ars.Common.Redis.Caching;
using Ars.Common.Redis.TypeCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis
{
    public static class IArsCacheProviderExtension
    {
        public static ITypeCache<TKey, TValue> AsType<TKey, TValue>(this IArsCacheProvider cacheProvider,string cachename) 
        {
            return new TypeCacheWrapper<TKey,TValue>(cacheProvider.GetArsCache(cachename));
        }
    }
}
