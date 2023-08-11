using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot.Extension
{
    public static class IServiceCollextionExtension
    {
        public static IArsWebApplicationBuilder AddArsOcelot(this IArsWebApplicationBuilder arsServiceBuilder) 
        {
            arsServiceBuilder.HostBuilder.ConfigureAppConfiguration((hostBuilderContext,configurationBuilder) =>
            {
                configurationBuilder
                    .AddJsonFile("ocelot.json", true, true)
                    .AddJsonFile($"ocelot.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", true, true);
            });

            var configuration = arsServiceBuilder.Configuration;
            var ocelotGlobalConfig = configuration.GetSection(nameof(FileConfiguration.GlobalConfiguration)).Get<FileGlobalConfiguration>();
            if (null == ocelotGlobalConfig)
            {
                throw new ArgumentNullException(nameof(FileConfiguration),"ocelot.json not be null");
            }

            var ocelotbuilder = arsServiceBuilder.Services.AddOcelot();
            if (ocelotGlobalConfig.ServiceDiscoveryProvider?.Type?.ToLower()?.Contains("consul") ?? false) 
            {
                ocelotbuilder.AddConsul();

                ocelotbuilder.Services.Replace(ServiceDescriptor.Singleton(ConsulProviderFactory.Get));
            }

            //添加swagger for ocelot
            arsServiceBuilder.Services.AddSwaggerForOcelot(configuration);

            var arsconfig = arsServiceBuilder.ServiceProvider
                .GetRequiredService<IArsConfiguration>();

            arsconfig.ArsOcelotConfiguration = new ArsOcelotConfiguration();
            arsconfig.AddArsAppExtension(new ArsOcelotAppExtension());

            return arsServiceBuilder;
        }
    }
}
