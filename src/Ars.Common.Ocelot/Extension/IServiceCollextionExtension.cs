using Ars.Commom.Core;
using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot.Extension
{
    public static class IServiceCollextionExtension
    {
        public static IArsWebApplicationBuilder AddArsOcelot(this IArsWebApplicationBuilder arsServiceBuilder,Action<IOcelotBuilder>? builderOption = null) 
        {
            arsServiceBuilder.HostBuilder.ConfigureAppConfiguration((hostBuilderContext,configurationBuilder) =>
            {
                configurationBuilder
                    .AddJsonFile("ocelot.json", true, true)
                    .AddJsonFile($"ocelot.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", true, true);
            });

            var configuration = arsServiceBuilder.Configuration;

            var ocelotGlobalConfig = configuration
                .GetSection(nameof(FileConfiguration.GlobalConfiguration))
                .Get<FileGlobalConfiguration>();

            if (null == ocelotGlobalConfig)
            {
                throw new ArgumentNullException(nameof(FileConfiguration),"ocelot.json not be null");
            }

            var ocelotbuilder = arsServiceBuilder.Services.AddOcelot();
            if (null != builderOption) 
            {
                builderOption(ocelotbuilder);
            }

            //consul
            if (ocelotGlobalConfig.ServiceDiscoveryProvider?.Type?.ToLower()?.Contains("consul") ?? false)
            {
                ocelotbuilder.AddConsul();

                ocelotbuilder.Services.Replace(ServiceDescriptor.Singleton(ConsulProviderFactory.Get));
            }

            var ocelotRoutesConfig = configuration
                .GetSection(nameof(FileConfiguration.Routes))
                .Get<List<FileRoute>>();

            if (ocelotRoutesConfig.HasNotValue())
            {
                throw new ArgumentNullException(nameof(FileConfiguration), "ocelot.json not be null");
            }

            //polly
            if (ocelotRoutesConfig.Any(r =>
                null != r.QoSOptions &&
                r.QoSOptions.TimeoutValue > 0 &&
                r.QoSOptions.ExceptionsAllowedBeforeBreaking > 0)) 
            {
                ocelotbuilder.AddArsPolly();
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
