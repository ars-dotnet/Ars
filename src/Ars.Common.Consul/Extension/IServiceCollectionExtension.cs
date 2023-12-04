using Ars.Commom.Core;
using Ars.Commom.Tool.Extension;
using Ars.Common.Tool.Extension;
using Ars.Common.Consul.Option;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Extensions;
using Ars.Common.Tool.Tools;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;

namespace Ars.Common.Consul.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsWebApplicationBuilder AddArsConsulDiscoverClient(
            this IArsWebApplicationBuilder arsServiceBuilder,
            IArsConfiguration? arsConfiguration = null)
        {
            if (null == arsConfiguration)
                throw new ArgumentNullException(nameof(arsConfiguration));

            var config = arsServiceBuilder.Configuration
                .GetSection(nameof(ConsulDiscoverConfiguration))
                .Get<ConsulDiscoverConfiguration>()
                ??
                throw new ArgumentNullException("appsettings => ConsulDiscoverConfiguration not be null");

            if (config.ConsulDiscovers.Any(r => null == r.Communication))
            {
                throw new ArgumentNullException("appsettings => ConsulDiscoverConfiguration.CommunicationConfiguration not be null");
            }

            var services = arsServiceBuilder.Services;

            services.AddSingleton<ConsulHelper>();

            foreach (var c in config.ConsulDiscovers.Where(r => r.Communication.UseHttps))
            {
                c.Communication.CertificatePath ??= arsConfiguration.ArsBasicConfiguration?.CertificatePath;
                c.Communication.CertificatePassWord ??= arsConfiguration.ArsBasicConfiguration?.CertificatePassWord;
            }

            arsConfiguration.ArsConsulDiscoverConfiguration ??= config;

            services.AddSingleton<IConsulDiscoverConfiguration>(_ => config);

            arsServiceBuilder
                .AddArsHttpClient()
                .AddArsGrpcHttpClient();

            services.AddMemoryCache();

            return arsServiceBuilder;
        }

        public static IArsWebApplicationBuilder AddArsConsulRegisterServer(this IArsWebApplicationBuilder arsServiceBuilder, IArsConfiguration? arsConfiguration = null)
        {
            if (null == arsConfiguration)
                throw new ArgumentNullException(nameof(arsConfiguration));

            var config = arsServiceBuilder.Configuration
                .GetSection(nameof(ConsulRegisterConfiguration))
                .Get<ConsulRegisterConfiguration>()
                ??
                throw new ArgumentNullException("appsettings => ConsulRegisterConfiguration not be null");

            if (arsConfiguration.ArsBasicConfiguration?.ServiceIp?.IsNullOrEmpty() ?? false)
            {
                throw new ArgumentNullException("appsettings => ArsBasicConfiguration.ServiceIp not be null");
            }
            if (null == arsConfiguration.ArsBasicConfiguration?.ServicePort || 0 == arsConfiguration.ArsBasicConfiguration?.ServicePort) 
            {
                throw new ArgumentNullException("appsettings => ArsBasicConfiguration.ServicePort not be null or zero");
            }

            config.ServiceIp ??= arsConfiguration!.ArsBasicConfiguration!.ServiceIp;
            config.ServicePort ??= arsConfiguration!.ArsBasicConfiguration!.ServicePort;
            config.CertificatePath ??= arsConfiguration!.ArsBasicConfiguration?.CertificatePath;
            config.CertificatePassWord ??= arsConfiguration!.ArsBasicConfiguration?.CertificatePassWord;

            arsConfiguration.ArsConsulRegisterConfiguration ??= config;

            var services = arsServiceBuilder.Services;
            services.AddSingleton<IConsulRegisterConfiguration>(_ => config);

            arsConfiguration.AddArsAppExtension(new ArsConsulAppExtension());

            return arsServiceBuilder;
        }

        public static IArsWebApplicationBuilder AddArsGrpcHttpClient(this IArsWebApplicationBuilder arsServiceBuilder)
        {
            var services = arsServiceBuilder.Services;

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
                    return policyBuilder.AddArsHttpClientPolicy();
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
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                    var grpchandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, handler)//https://github.com/grpc/grpc-dotnet/issues/1110
                    {
                        HttpVersion = new Version(1, 1)
                    };

                    return grpchandler;
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.AddArsHttpClientPolicy();
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
                    return policyBuilder.AddArsHttpClientPolicy();
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
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                    return handler;
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.AddArsHttpClientPolicy();
                });
            #endregion

            return arsServiceBuilder;
        }
    }
}
