using Ars.Commom.Tool.Certificates;
using Ars.Common.Consul.HttpClientHelper;
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
    internal class GrpcClientProvider : IGrpcClientProvider, ISingletonDependency
    {
        private readonly ConsulHelper _consulHelper;
        private readonly IConsulDiscoverConfiguration _options;
        private readonly IGrpcMetadataTokenProvider _grpcCallOptionsProvider;
        private readonly IHttpClientProvider _httpClientProvider;
        public GrpcClientProvider(
            ConsulHelper consulHelper,
            IConsulDiscoverConfiguration options,
            IGrpcMetadataTokenProvider grpcCallOptionsProvider,
            IHttpClientProvider httpClientProvider)
        {
            _consulHelper = consulHelper;
            _options = options;
            _grpcCallOptionsProvider = grpcCallOptionsProvider;
            _httpClientProvider = httpClientProvider;
        }

        public virtual async Task<T> GetGrpcClient<T>(string serviceName) where T : ClientBase<T>
        {
            var option = _options.ConsulDiscovers.
                   FirstOrDefault(r => r.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArsException($"consul service:{serviceName} not find");

            return (T)Activator.CreateInstance(typeof(T), await GetGrpcCallInvoker(option))!;
        }

        private async Task<CallInvoker> GetGrpcCallInvoker(ConsulConfiguration configuration)
        {
            GrpcChannelOptions channelOptions = await GetGrpcChannelOptions(configuration);
            var channel = GrpcChannel.ForAddress(channelOptions.HttpClient!.BaseAddress!, channelOptions);
            var callInvoker = channel.Intercept(
                new GrpcClientTokenInterceptor(configuration, _grpcCallOptionsProvider));

            return callInvoker;
        }

        private async Task<GrpcChannelOptions> GetGrpcChannelOptions(ConsulConfiguration configuration)
        {
            return new GrpcChannelOptions
            {
                HttpClient = await _httpClientProvider.GetGrpcHttpClientAsync<HttpClient>(configuration)
            };
        }

        /// <summary>
        /// 建议采用IHttpClientFactory来创建
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [Obsolete]
        private HttpClient GetHttpClient(ConsulConfiguration config)
        {
            var configuration = config.Communication;

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
            if (configuration.GrpcUseHttp1Protocol)
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
