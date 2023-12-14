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
            this IArsWebApplicationBuilder arsServiceBuilder)
        {
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
                c.Communication.CertificatePath ??= arsServiceBuilder.ArsConfiguration.ArsBasicConfiguration?.CertificatePath;
                c.Communication.CertificatePassWord ??= arsServiceBuilder.ArsConfiguration.ArsBasicConfiguration?.CertificatePassWord;
            }

            arsServiceBuilder.ArsConfiguration.ArsConsulDiscoverConfiguration ??= config;

            services.AddSingleton<IConsulDiscoverConfiguration>(_ => config);

            arsServiceBuilder
                .AddArsHttpClient()
                .AddArsGrpcHttpClient();

            services.AddMemoryCache();

            return arsServiceBuilder;
        }

        public static IArsWebApplicationBuilder AddArsConsulRegisterServer(this IArsWebApplicationBuilder arsServiceBuilder)
        {
            var config = arsServiceBuilder.Configuration
                .GetSection(nameof(ConsulRegisterConfiguration))
                .Get<ConsulRegisterConfiguration>()
                ??
                throw new ArgumentNullException("appsettings => ConsulRegisterConfiguration not be null");

            if (arsServiceBuilder.ArsConfiguration.ArsBasicConfiguration?.ServiceIp?.IsNullOrEmpty() ?? false)
            {
                throw new ArgumentNullException("appsettings => ArsBasicConfiguration.ServiceIp not be null");
            }
            if (null == arsServiceBuilder.ArsConfiguration.ArsBasicConfiguration?.ServicePort ||
                0 == arsServiceBuilder.ArsConfiguration.ArsBasicConfiguration?.ServicePort) 
            {
                throw new ArgumentNullException("appsettings => ArsBasicConfiguration.ServicePort not be null or zero");
            }

            config.ServiceIp ??= arsServiceBuilder.ArsConfiguration!.ArsBasicConfiguration!.ServiceIp;
            config.ServicePort ??= arsServiceBuilder.ArsConfiguration!.ArsBasicConfiguration!.ServicePort;
            config.CertificatePath ??= arsServiceBuilder.ArsConfiguration!.ArsBasicConfiguration?.CertificatePath;
            config.CertificatePassWord ??= arsServiceBuilder.ArsConfiguration!.ArsBasicConfiguration?.CertificatePassWord;

            arsServiceBuilder.ArsConfiguration.ArsConsulRegisterConfiguration ??= config;

            var services = arsServiceBuilder.Services;
            services.AddSingleton<IConsulRegisterConfiguration>(_ => config);

            arsServiceBuilder.ArsConfiguration.AddArsAppExtension(new ArsConsulAppExtension());

            return arsServiceBuilder;
        }

        public static IArsWebApplicationBuilder AddArsGrpcHttpClient(this IArsWebApplicationBuilder arsServiceBuilder)
        {
            var services = arsServiceBuilder.Services;

            #region grpchttpclient
            services
                .AddHttpClient(HttpClientNames.RetryGrpcHttpV1)
                .ConfigurePrimaryHttpMessageHandler(_ =>
                {
                    //https://github.com/grpc/grpc-dotnet/issues/1110
                    var grpchandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb,new HttpClientHandler())
                    {
                        HttpVersion = new Version(1, 1),
                    };

                    return grpchandler;
                })
                .AddArsTransientHttpErrorPolicy();

            services
                .AddHttpClient(HttpClientNames.RetryGrpcHttpsV1)
                .ConfigureArsGrpcPrimaryHttpsMessageHandler(handler => 
                {
                    //https://github.com/grpc/grpc-dotnet/issues/1110
                    var grpchandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, handler)
                    {
                        HttpVersion = new Version(1, 1)
                    };

                    return grpchandler;
                })
                .AddArsTransientHttpErrorPolicy();
            #endregion

            return arsServiceBuilder;
        }
    }
}
