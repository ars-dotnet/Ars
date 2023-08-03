using Ars.Commom.Core;
using Ars.Commom.Tool.Serializer;
using Ars.Common.AutoFac.Dependency;
using Ars.Common.AutoFac.Extension;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Ars.Common.AutoFac.RegisterProvider;
using Ars.Common.Core;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Core.Configs;
using Ars.Common.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Host.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArserviceCore(
            this IServiceCollection services,
            WebApplicationBuilder builder,
            Action<IArsConfiguration>? action = null)
        {
            var arsbuilder = new ArsServiceBuilder(services, builder.Host, builder.Configuration);
            services.AddSingleton<IArsSerializer, ArsSerializer>();
            services.AddSingleton<IArsConfiguration>(new ArsConfiguration());

            action ??= _ => Console.WriteLine("Default Ars Start");
            action += r => r.AddArsAspNetCore();
            action += r => r.AddArsAutofac();

            using var scope = services.BuildServiceProvider().CreateScope();
            IArsConfiguration arsConfig = scope.ServiceProvider.GetRequiredService<IArsConfiguration>();
            action?.Invoke(arsConfig);

            ArsBasicConfiguration arsBasicConfiguration =
                scope.ServiceProvider.GetRequiredService<IConfiguration>()
                .GetSection(nameof(ArsBasicConfiguration))
                .Get<ArsBasicConfiguration>() ?? new ArsBasicConfiguration();
            services.AddSingleton<IOptions<IArsBasicConfiguration>>(
                new OptionsWrapper<IArsBasicConfiguration>(arsBasicConfiguration));

            arsConfig.ArsBasicConfiguration = arsBasicConfiguration;

            foreach (var serviceExtension in arsConfig.ArsServiceExtensions)
            {
                serviceExtension.AddService(arsbuilder);
            }

            return arsbuilder;
        }
    }
}
