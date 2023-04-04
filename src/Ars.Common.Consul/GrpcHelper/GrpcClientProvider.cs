using Ars.Commom.Tool.Certificates;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Tool;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.GrpcHelper
{
    internal class GrpcClientProvider : IGrpcClientProvider,ISingletonDependency
    {
        private readonly ConsulHelper ConsulHelper;
        private readonly IConsulDiscoverConfiguration _options;
        private readonly IGrpcMetadataTokenProvider _grpcCallOptionsProvider;
        public GrpcClientProvider(
            ConsulHelper consulHelper, 
            IConsulDiscoverConfiguration options,
            IGrpcMetadataTokenProvider grpcCallOptionsProvider)
        {
            ConsulHelper = consulHelper;
            _options = options;
            _grpcCallOptionsProvider = grpcCallOptionsProvider;
        }

        public virtual async Task<T> GetGrpcClient<T>(string serviceName) where T : ClientBase<T>
        {
            var option = _options.ConsulDiscovers.
                   FirstOrDefault(r => r.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArsException($"consul service:{serviceName} not find");

            string domain = await ConsulHelper.GetServiceDomain(serviceName, option.ConsulAddress);
            if (option.UseHttps)
                domain = domain.Replace("http", "https");
            return (T)Activator.CreateInstance(typeof(T),GetGrpcCallInvoker(domain, option))!;
        }

        private CallInvoker GetGrpcCallInvoker(string domain, ConsulConfiguration configuration) 
        {
            var channel = GrpcChannel.ForAddress(domain, GetGrpcChannelOptions(configuration));
            var callInvoker = channel.Intercept(
                new GrpcClientTokenInterceptor(configuration, _grpcCallOptionsProvider));

            return callInvoker;
        }

        private GrpcChannelOptions GetGrpcChannelOptions(ConsulConfiguration configuration) 
        {
            return new GrpcChannelOptions { HttpClient = GetHttpClient(configuration) };
        }

        private HttpClient GetHttpClient(ConsulConfiguration configuration) 
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12,
            };

            if (configuration.UseHttps) 
            {
                handler.ClientCertificates.Add(
                    Certificate.Get(configuration.CertificatePath, configuration.CertificatePassWord));
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }

            HttpClient httpClient;
            if (configuration.UseHttp1Protocol)
            {
                var grpchandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, handler)//https://github.com/grpc/grpc-dotnet/issues/1110
                {
                    HttpVersion = new Version(1, 1)
                };

                httpClient = new HttpClient(grpchandler)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
            }
            else 
            {
                httpClient = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
            }
            return httpClient;
        }
    }
}
