using Ars.Common.Tool.Export;
using Microsoft.Extensions.DependencyInjection;
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
        /// http client services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddArsHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient();
            services
                .AddHttpClient("Https")
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
                .AddHttpClient("RetryHttp")
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
                });
            services
                .AddHttpClient("RetryHttps")
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
        /// export services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddArsExportService(this IServiceCollection services,Assembly assembly)
        {
            var provider = new ExportApiSchemeProvider();
            services.AddSingleton<IExportApiSchemeProvider>(provider);
            services.AddScoped<IExportManager, ExportManager>();
            services.AddSingleton<IXmlFileManager, XmlFileManager>();

            provider.SetExportApiSchemed(assembly);

            return services;
        }
    }
}
