using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ars.Common.Core.Localization.Extension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsLocalization(this IApplicationBuilder applicationBuilder) 
        {
            var options = applicationBuilder.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            applicationBuilder.UseRequestLocalization(options!.Value);

            return applicationBuilder;
        }
    }
}
