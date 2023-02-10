using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis
{
    public class ArsCacheOption
    {
        public string RedisConnection { get; set; }

        public int DefaultDB { get; set; }

        public int IdleTimeout { get; set; } = 3000;

        public int Poolsize { get; set; } = 5;
    }
}
