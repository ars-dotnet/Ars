using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;

namespace Ars.Common.Tool.Extension
{
    public static class IServiceCollectionExtension
    {
        public static void AddArsHttpClient(this IServiceCollection services)
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
        }
    }
}
