using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Core.Configs;
using Ars.Common.IdentityServer4.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ars.Commom.Host.Extension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsCore(this IApplicationBuilder applicationBuilder) 
        {
            var config = applicationBuilder.ApplicationServices.GetRequiredService<IArsConfiguration>();

            //consul
            if (null != config.ConsulRegisterConfiguration) 
            {
                applicationBuilder.UseArsConsul(config.ConsulRegisterConfiguration);
            }

            //IdentityServer4 client
            if (null != config.ArsIdentityClientConfiguration) 
            {
                applicationBuilder.UseArsIdentityClient();
            }
            //IdentityServer4 server
            if (null != config.ArsIdentityServerConfiguration) 
            {
                applicationBuilder.UseArsIdentityServer();
            }

            return applicationBuilder;
        }
    }
}
