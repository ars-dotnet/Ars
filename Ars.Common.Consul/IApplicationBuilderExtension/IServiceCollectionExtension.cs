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
        public static IServiceCollection AddArsConsul(this IServiceCollection services,IConfiguration configuration) 
        {
            services.Configure<ConsulOption>(configuration.GetSection(nameof(ConsulOption)));
            return services;
        }
    }
}
