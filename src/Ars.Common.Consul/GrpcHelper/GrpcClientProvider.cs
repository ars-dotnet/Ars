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
    internal class GrpcClientProvider : IGrpcClientProviderByConsul, ISingletonDependency
    {
        private readonly IConsulDiscoverConfiguration _options;
        private readonly IGrpcMetadataTokenProvider _grpcCallOptionsProvider;
        private readonly IHttpClientProviderByConsul _httpClientProvider;
        public GrpcClientProvider(
            IConsulDiscoverConfiguration options,
            IGrpcMetadataTokenProvider grpcCallOptionsProvider,
            IHttpClientProviderByConsul httpClientProvider)
        {
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
    }
}
