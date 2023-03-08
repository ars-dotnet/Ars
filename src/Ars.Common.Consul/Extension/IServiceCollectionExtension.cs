﻿using Ars.Commom.Core;
using Ars.Common.Consul.Option;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsConsulDiscoverClient(this IArsServiceBuilder arsServiceBuilder) 
        {
            var services = arsServiceBuilder.Services.ServiceCollection;
            services.AddSingleton<ConsulHelper>();

            var config = arsServiceBuilder.Services.Provider
                .GetRequiredService<IConfiguration>()
                .GetSection(nameof(ConsulDiscoverConfiguration))
                .Get<ConsulDiscoverConfiguration>()
                ??
                throw new ArgumentNullException("appsettings => ConsulDiscoverConfiguration not be null");
            arsServiceBuilder.Services.Provider
                .GetRequiredService<IArsConfiguration>()
                .ConsulDiscoverConfiguration ??= config;

            services.AddSingleton<IConsulDiscoverConfiguration>(config);

            return arsServiceBuilder;
        }

        public static IArsServiceBuilder AddArsConsulRegisterServer(this IArsServiceBuilder arsServiceBuilder) 
        {
            var config = arsServiceBuilder.Services.Provider
                .GetRequiredService<IConfiguration>()
                .GetSection(nameof(ConsulRegisterConfiguration))
                .Get<ConsulRegisterConfiguration>()
                ??
                throw new ArgumentNullException("appsettings => ConsulRegisterConfiguration not be null");

            var arscfg = arsServiceBuilder.Services.Provider
                .GetRequiredService<IArsConfiguration>();
            arscfg.ConsulRegisterConfiguration ??= config;
            arsServiceBuilder.Services.ServiceCollection.AddSingleton<IConsulRegisterConfiguration>(config);

            arscfg.AddArsAppExtension(new ArsConsulAppExtension());

            return arsServiceBuilder;
        }

    }
}