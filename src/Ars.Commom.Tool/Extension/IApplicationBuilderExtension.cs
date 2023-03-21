using Ars.Common.Tool.Configs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsUploadExcel(this IApplicationBuilder builder) 
        {
            var basiconfig = builder.ApplicationServices.GetRequiredService<IOptions<IArsBasicConfiguration>>();
            var config = builder.ApplicationServices.GetRequiredService<IOptions<IArsUploadExcelConfiguration>>();

            string path = Path.Combine(basiconfig.Value.Root, config.Value.UploadRoot);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            builder.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(path),
                RequestPath = "/" + config.Value.RequestPath,
            });

            return builder;
        }
    }
}
