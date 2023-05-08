using Ars.Common.Tool.Configs;
using Ars.Common.Tool.Export;
using Ars.Common.Tool.Tools;
using Ars.Common.Tool.UploadExcel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;
using System.Text;

namespace Ars.Common.Tool.Extension
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// add http client services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddArsHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient(HttpClientNames.Http);
            services
                .AddHttpClient(HttpClientNames.Https)
                .ConfigurePrimaryHttpMessageHandler((e) =>
                {
                    var handler = new HttpClientHandler();
                    handler.AllowAutoRedirect = true;
                    handler.UseCookies = true;
                    handler.CookieContainer = new CookieContainer();
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return handler;
                });

            services
                .AddHttpClient(HttpClientNames.RetryHttp)
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
                });
            services
                .AddHttpClient(HttpClientNames.RetryHttps)
                .ConfigurePrimaryHttpMessageHandler((e) =>
                {
                    var handler = new HttpClientHandler();
                    handler.AllowAutoRedirect = true;
                    handler.UseCookies = true;
                    handler.CookieContainer = new CookieContainer();
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return handler;
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
                });

            return services;
        }

        /// <summary>
        /// add export excel services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddArsExportExcelService(this IServiceCollection services,Assembly assembly)
        {
            var provider = new ExportApiSchemeProvider();
            services.AddScoped<IExportManager, ExportManager>();
            services.AddSingleton<IExportApiSchemeProvider>(provider);
            services.AddSingleton<IXmlFileManager, XmlFileManager>();

            provider.SetExportApiSchemed(assembly);

            return services;
        }

        /// <summary>
        /// add upload excel services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddArsUploadExcelService(this IServiceCollection services,Action<IArsUploadExcelConfiguration> action) 
        {
            IArsUploadExcelConfiguration config = new ArsUploadExcelConfiguration();
            action(config);
            services.AddSingleton<IOptions<IArsUploadExcelConfiguration>>(new OptionsWrapper<IArsUploadExcelConfiguration>(config));

            services.AddScoped<IExcelResolve, ExcelResolve>();
            services.AddScoped<IExcelStorage, ExcelStorage>();
            services.AddScoped<ArsModelBinder>();

            services.AddTransient<ArsExcelActionFilter>();

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new ArsModelBinderProvider());
                options.Filters.Add<ArsExcelActionFilter>();
            });

            return services;
        }
    }
}
