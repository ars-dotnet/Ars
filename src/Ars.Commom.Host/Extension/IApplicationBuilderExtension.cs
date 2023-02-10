using Ars.Common.Core.Configs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ars.Commom.Host.Extension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsCore(this IApplicationBuilder applicationBuilder) 
        {
            var config = applicationBuilder.ApplicationServices.GetRequiredService<IArsConfiguration>();

            foreach (var appext in config.ArsAppExtensions)
            {
                appext.UseApplication(applicationBuilder, config);
            }

            return applicationBuilder;
        }
    }
}
