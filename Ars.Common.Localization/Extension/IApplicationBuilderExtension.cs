using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ars.Common.Localization.Extension
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
