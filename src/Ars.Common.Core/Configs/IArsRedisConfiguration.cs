using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IArsRedisConfiguration
    {
        string RedisConnection { get; set; }

        int DefaultDB { get; set; }

        int IdleTimeout { get; set; } 

        int Poolsize { get; set; }
    }
}
