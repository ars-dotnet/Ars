using Ars.Commom.Core;
using Ars.Common.Consul.Option;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.IApplicationBuilderExtension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsConsulRegisterServer(this IArsServiceBuilder arsServiceBuilder) 
        {
            var config = arsServiceBuilder.Services.Provider
                .GetRequiredService<IConfiguration>()
                .GetSection(nameof(ConsulRegisterConfiguration))
                .Get<ConsulRegisterConfiguration>() 
                ?? 
                throw new ArgumentNullException("appsettings => ConsulRegisterConfiguration not be null");

            arsServiceBuilder.Services.Provider
                .GetRequiredService<IArsConfiguration>()
                .ConsulRegisterConfiguration ??= config;
            arsServiceBuilder.Services.ServiceCollection.AddSingleton<IConsulRegisterConfiguration>(config);
            return arsServiceBuilder;
        }

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
    }
}
