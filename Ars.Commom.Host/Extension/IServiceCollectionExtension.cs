using Ars.Commom.Core;
using Ars.Common.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Host.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArserviceCore(this IServiceCollection services, WebApplicationBuilder builder) 
        {
            var buildServiceProvider = services.BuildServiceProvider();
            var configuration = buildServiceProvider.GetRequiredService<IConfiguration>();
            var webHostEnvironment = buildServiceProvider.GetRequiredService<IWebHostEnvironment>();
           
            var arsprovider = new ArsServiceBuilder(new ArsServiceCollection(services,configuration,webHostEnvironment),builder.Host);
            arsprovider.AddArsAutofac();
            arsprovider.HostBuilder.UseServiceProviderFactory(new ArsServiceProviderFactory(arsprovider.Services.Provider));

            return arsprovider;
        }
    }
}
