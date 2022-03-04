using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Caching
{
    public class ArsRedisCacheDatabaseProvider : IArsRedisCacheDatabaseProvider
    {
        public ArsRedisCacheDatabaseProvider()
        {
            
        }

        public IDatabase GetDatabase()
        {
            return null;
        }
    }
}
