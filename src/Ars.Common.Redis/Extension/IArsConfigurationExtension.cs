using Ars.Common.Core.Configs;
using Ars.Common.Redis.CacheConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis.Extension
{
    public static class IArsConfigurationExtension
    {
        public static void AddArsRedis(
            this IArsConfiguration arsConfiguration, 
            Action<ICacheConfigurationProvider>? config = null) 
        {
            arsConfiguration.AddArsServiceExtension(new ArsRedisServiceExtension(config));
        }
    }
}
