using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyArsenal.Commom.Tool
{
    public static class BaseConfigExtension
    {
        public static IServiceCollection AddArsConfig(this IServiceCollection services, Action<ArsBaseConfig> action)
        {
            if (null == action)
                throw new ArgumentNullException(nameof(action));

            ArsBaseConfig config = new ArsBaseConfig();
            action(config);

            foreach (var e in config.configExtensions)
            {
                e.AddService(services);
            }

            services.Configure(action);
            return services;
        }
    }
}
