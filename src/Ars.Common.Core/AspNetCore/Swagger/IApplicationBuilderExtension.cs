using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.Swagger
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsSwaggerUI(this IApplicationBuilder builder)
        {
            Action<SwaggerUIOptions> setupAction = options =>
            {
                var apiVersionDescriptionProvider = builder.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/Api/ArsWebApi/swagger/{description.GroupName}/swagger.json", 
                        description.GroupName.ToUpperInvariant());
                }
            };

            return builder.UseArsSwaggerUI(setupAction);
        }

        public static IApplicationBuilder UseArsSwaggerUI(this IApplicationBuilder builder, Action<SwaggerUIOptions>? setupAction = null)
        {
            return builder.UseSwaggerUI(setupAction);
        }
    }
}
