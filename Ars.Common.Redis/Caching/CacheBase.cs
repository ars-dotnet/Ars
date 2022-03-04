using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public abstract class CacheBase : ArsCacheBase<string, object>, ICache
    {
        protected CacheBase(string name) : base(name)
        {
            
        }
    }
}
