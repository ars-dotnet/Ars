using Ars.Commom.Core;
using Ars.Common.Tool.Tools;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Extensions
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// add http client services
        /// </summary>
        /// <param name="arsServiceBuilder"></param>
        /// <returns></returns>
        public static IArsServiceBuilder AddArsHttpClient(this IArsServiceBuilder arsServiceBuilder)
        {
            var services = arsServiceBuilder.Services;

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

            return arsServiceBuilder;
        }

    }
}
