using Ars.Common.Redis.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.TypeCache
{
    public interface ITypeCache<TKey,TValue> : IDisposable, IArsCacheOption, IArsCache<TKey, TValue>
    {
        ICache InnerCache { get; }
    }
}
