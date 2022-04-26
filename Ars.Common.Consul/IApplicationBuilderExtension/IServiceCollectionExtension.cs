using Ars.Commom.Core;
using Ars.Common.Consul.Option;
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
        public static IArsServiceBuilder AddArsConsulRegister(this IArsServiceBuilder arsServiceBuilder, [NotNull]Action<ConsulRegisteOption> action) 
        {
            arsServiceBuilder.Services.ServiceCollection.Configure(action);
            return arsServiceBuilder;
        }

        public static IArsServiceBuilder AddArsConsulDiscover(this IArsServiceBuilder arsServiceBuilder, [NotNull] Action<ConsulDiscoverOption> action) 
        {
            var services = arsServiceBuilder.Services.ServiceCollection;
            services.AddSingleton<ConsulHelper>();
            services.Configure(action);
            return arsServiceBuilder;
        }
    }
}
