using Ars.Common.Consul.Option;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.IApplicationBuilderExtension
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddArsConsulRegister(this IServiceCollection services,IConfiguration configuration) 
        {
            services.Configure<ConsulRegisteOption>(configuration.GetSection(nameof(ConsulRegisteOption)));
            return services;
        }

        public static IServiceCollection AddArsConsulDiscover(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddSingleton<ConsulHelper>();
            services.Configure<ConsulDiscoverOption>(configuration.GetSection(nameof(ConsulDiscoverOption)));
            return services;
        }
    }
}
