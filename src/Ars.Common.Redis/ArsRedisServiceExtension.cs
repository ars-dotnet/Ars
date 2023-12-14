using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Redis.CacheConfiguration;
using Ars.Common.Redis.RedisExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Redis
{
    internal class ArsRedisServiceExtension : IArsServiceExtension
    {
        private readonly Action<ICacheConfigurationProvider>? _config;
        public ArsRedisServiceExtension(Action<ICacheConfigurationProvider>? config)
        {
            _config = config;
        }

        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsRedis(_config);
        }
    }
}
