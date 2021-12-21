using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyArsenal.Commom.Tool
{
    public static class BaseConfigExtension
    {
        public static IServiceCollection AddMyArsenal(this IServiceCollection services,Action<MyArsenalBaseConfig> action)
        {
            if (null == action)
                throw new ArgumentNullException(nameof(action));

            MyArsenalBaseConfig config = new MyArsenalBaseConfig();
            action(config);

            foreach (var e in config.configExtensions) 
            {
                e.AddService(services);
            }

            services.AddOptions();
            if (null != action)
                services.Configure(action);

            return services;
        }
    }
}
