using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public interface ICache : IDisposable, IArsCacheOption, IArsCache<string, object>
    {

    }
}
