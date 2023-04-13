using Ars.Common.Tool.Configs;
using Ars.Common.Tool.Export;
using Ars.Common.Tool.Tools;
using Ars.Common.Tool.UploadExcel;
using Grpc.Net.Client.Web;
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

            #region grpchttpclient
            services
                .AddHttpClient(HttpClientNames.RetryGrpcHttpV1)
                .ConfigurePrimaryHttpMessageHandler(e => 
                {
                    var handler = new HttpClientHandler
                    {
                        SslProtocols = SslProtocols.Tls12,
                    };
                    var grpchandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, handler)//https://github.com/grpc/grpc-dotnet/issues/1110
                    {
                        HttpVersion = new Version(1, 1),
                    };

                    return grpchandler;
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
                });
            services
                .AddHttpClient(HttpClientNames.RetryGrpcHttpsV1)
                .ConfigurePrimaryHttpMessageHandler((e) =>
                {
                    var handler = new HttpClientHandler();
                    handler.AllowAutoRedirect = true;
                    handler.UseCookies = true;
                    handler.CookieContainer = new CookieContainer();
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

                    var grpchandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, handler)//https://github.com/grpc/grpc-dotnet/issues/1110
                    {
                        HttpVersion = new Version(1, 1)
                    };

                    return grpchandler;
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
                });

            services
                .AddHttpClient(HttpClientNames.RetryGrpcHttpV2)
                .ConfigurePrimaryHttpMessageHandler(e =>
                {
                    var handler = new HttpClientHandler
                    {
                        SslProtocols = SslProtocols.Tls12,
                    };

                    return handler;
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
                });
            services
                .AddHttpClient(HttpClientNames.RetryGrpcHttpsV2)
                .ConfigurePrimaryHttpMessageHandler((e) =>
                {
                    var handler = new HttpClientHandler();
                    handler.AllowAutoRedirect = true;
                    handler.UseCookies = true;
                    handler.CookieContainer = new CookieContainer();
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    handler.SslProtocols = SslProtocols.Tls12;
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

                    return handler;
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });
                });
            #endregion

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
