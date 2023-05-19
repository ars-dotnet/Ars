using Ars.Commom.Core;
using Ars.Common.SignalR.Caches;
using Ars.Common.SignalR.Hubs;
using Ars.Common.SignalR.Sender;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Ars.Common.SignalR.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsSignalR(this IArsServiceBuilder arsServiceBuilder,Action<ArsSignalRConfiguration>? action = null) 
        {
            ArsSignalRConfiguration config = new ArsSignalRConfiguration();
            action?.Invoke(config);

            var service = arsServiceBuilder.Services;
            if (config.CacheType == 0)
                service.AddScoped<IHubCacheManager, MemoryHubCacheManager>();
            else
                service.AddScoped<IHubCacheManager, RedisHubCacheManager>();

            service.AddSingleton<IHubSenderProvider, HubSenderProvider>();

            service.AddScoped<IHubSendMessage, ArsWebHub>();
            service.AddScoped<IHubSendMessage, ArsAndroidHub>();

            if (config.UseMessagePackProtocol)
                service.AddSignalR().AddMessagePackProtocol();
            else
                service.AddSignalR();

            return arsServiceBuilder;
        }
    }
}
