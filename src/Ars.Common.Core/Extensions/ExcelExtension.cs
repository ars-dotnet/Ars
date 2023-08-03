using Ars.Commom.Core;
using Ars.Common.Core.Excels.ExportExcel;
using Ars.Common.Core.Excels.UploadExcel;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace Ars.Common.Core.Extensions
{
    public static class ExcelExtension
    {
        /// <summary>
        /// add export excel services
        /// </summary>
        /// <param name="arsServiceBuilder"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IArsServiceBuilder AddArsExportExcelService(this IArsServiceBuilder arsServiceBuilder, Assembly assembly)
        {
            var services = arsServiceBuilder.Services;
            services.AddScoped<IExportManager, ExportManager>();

            var provider = new ExportApiSchemeProvider();
            services.AddSingleton<IExportApiSchemeProvider>(provider);
            services.AddSingleton<IXmlFileManager, XmlFileManager>();

            provider.SetExportApiSchemed(assembly);

            return arsServiceBuilder;
        }

        /// <summary>
        /// add upload excel services
        /// </summary>
        /// <param name="arsServiceBuilder"></param>
        /// <returns></returns>
        public static IArsServiceBuilder AddArsUploadExcelService(this IArsServiceBuilder arsServiceBuilder, Action<IArsUploadExcelConfiguration> action)
        {
            IArsUploadExcelConfiguration config = new ArsUploadExcelConfiguration();
            action(config);

            var services = arsServiceBuilder.Services;
            services.AddSingleton<IOptions<IArsUploadExcelConfiguration>>(
                new OptionsWrapper<IArsUploadExcelConfiguration>(config));

            services.AddScoped<IExcelResolve, ExcelResolve>();
            services.AddScoped<IExcelStorage, ExcelStorage>();
            services.AddScoped<ArsModelBinder>();

            services.AddTransient<ArsExcelActionFilter>();

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new ArsModelBinderProvider());
                options.Filters.Add<ArsExcelActionFilter>();
            });

            return arsServiceBuilder;
        }

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
