using Ars.Commom.Core;
using Ars.Common.Consul.Option;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Tool.Extension;
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
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsConsulDiscoverClient(this IArsServiceBuilder arsServiceBuilder)
        {
            var services = arsServiceBuilder.Services;
            services.AddSingleton<ConsulHelper>();

            var config = arsServiceBuilder.Configuration
                .GetSection(nameof(ConsulDiscoverConfiguration))
                .Get<ConsulDiscoverConfiguration>()
                ??
                throw new ArgumentNullException("appsettings => ConsulDiscoverConfiguration not be null");

            if (config.ConsulDiscovers.Any(r => null == r.Communication))
            {
                throw new ArgumentNullException("appsettings => ConsulDiscoverConfiguration.CommunicationConfiguration not be null");
            }

            arsServiceBuilder.ServiceProvider
                 .GetRequiredService<IArsConfiguration>()
                .ArsConsulDiscoverConfiguration ??= config;

            services.AddSingleton<IConsulDiscoverConfiguration>(config);

            services
                .AddArsHttpClient()
                .AddArsGrpcHttpClient()
                .AddMemoryCache();

            return arsServiceBuilder;
        }

        public static IArsServiceBuilder AddArsConsulRegisterServer(this IArsServiceBuilder arsServiceBuilder)
        {
            var services = arsServiceBuilder.Services;
            var config = arsServiceBuilder.Configuration
                .GetSection(nameof(ConsulRegisterConfiguration))
                .Get<ConsulRegisterConfiguration>()
                ??
                throw new ArgumentNullException("appsettings => ConsulRegisterConfiguration not be null");

            var arscfg = arsServiceBuilder.ServiceProvider
                .GetRequiredService<IArsConfiguration>();
            arscfg.ArsConsulRegisterConfiguration ??= config;
            services.AddSingleton<IConsulRegisterConfiguration>(config);

            arscfg.AddArsAppExtension(new ArsConsulAppExtension());

            return arsServiceBuilder;
        }

        public static IServiceCollection AddArsGrpcHttpClient(this IServiceCollection services)
        {
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
    }
}
