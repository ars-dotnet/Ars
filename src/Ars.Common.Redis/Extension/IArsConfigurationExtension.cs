using Ars.Common.Core.Configs;
using Ars.Common.Redis.CacheConfiguration;

namespace Ars.Common.Redis.Extension
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsRedis(
            this IArsConfiguration arsConfiguration, 
            Action<ICacheConfigurationProvider>? config = null) 
        {
            arsConfiguration.AddArsServiceExtension(new ArsRedisServiceExtension(config));

            return arsConfiguration;
        }
    }
}
