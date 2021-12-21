using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;

namespace MyArsenal.Commom.Tool.ServiceExtension
{
    public static class HttpClientsExt
    {
        public static void AddHcdHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpClient("Https").ConfigurePrimaryHttpMessageHandler((e) =>
            {
                var handler = new HttpClientHandler();
                handler.AllowAutoRedirect = true;
                handler.UseCookies = true;
                handler.CookieContainer = new CookieContainer();
                handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                handler.ServerCertificateCustomValidationCallback = (HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors) => true;
                return handler;
            });
            services.AddHttpClient("Https_add").ConfigurePrimaryHttpMessageHandler((e) =>
            {
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true,
                    UseCookies = true,
                    CookieContainer = new CookieContainer(),
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                    ClientCertificateOptions = ClientCertificateOption.Automatic,
                    ServerCertificateCustomValidationCallback = (HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors) => true
                };

                return handler;
            });

            services.AddHttpClient("swagger", (h) =>
            {
                h.Timeout = TimeSpan.FromMinutes(10);
            });

        }
    }
}
