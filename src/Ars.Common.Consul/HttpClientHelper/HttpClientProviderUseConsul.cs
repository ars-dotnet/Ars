using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Tool;
using Ars.Common.Tool.Tools;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.HttpClientHelper
{
    internal class HttpClientProvider : IHttpClientProviderByConsul, ISingletonDependency
    {
        private readonly ConsulHelper _consulHelper;
        private readonly IConsulDiscoverConfiguration _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IToken _token;
        public HttpClientProvider(
            ConsulHelper consulHelper,
            IConsulDiscoverConfiguration option,
            IHttpClientFactory httpClientFactory,
            IToken token)
        {
            _consulHelper = consulHelper;
            _options = option;
            _httpClientFactory = httpClientFactory;
            _token = token;
        }

        public Task<T> GetHttpClientAsync<T>(string serviceName) where T : HttpClient
        {
            var option = _options.ConsulDiscovers.
                   FirstOrDefault(r => r.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArsException($"consul service:{serviceName} not find");

            return GetHttpClientAsync<T>(option);
        }

        public Task<T> GetHttpClientAsync<T>(ConsulConfiguration config) where T : HttpClient
        {
            return GetClient<T>(config, HttpClientNames.RetryHttp);
        }

        public Task<T> GetGrpcHttpClientAsync<T>(ConsulConfiguration config) where T : HttpClient
        {
            return GetClient<T>(config, HttpClientNames.RetryGrpcHttpV2, grpc: true);
        }

        private async Task<T> GetClient<T>(ConsulConfiguration config, string httpClientName, bool grpc = false) where T : HttpClient
        {
            string domain = await _consulHelper.GetServiceDomain(config.ServiceName, config.ConsulAddress);

            if (grpc)
            {
                if (config.Communication.GrpcUseHttp1Protocol)
                {
                    if (config.Communication.UseHttps)
                    {
                        domain = domain.Replace("http", "https");
                        httpClientName = HttpClientNames.RetryGrpcHttpsV1;
                    }
                    else
                    {
                        httpClientName = HttpClientNames.RetryGrpcHttpV1;
                    }
                }
                else if (config.Communication.UseHttps)
                {
                    domain = domain.Replace("http", "https");
                    httpClientName = HttpClientNames.RetryGrpcHttpsV2;
                }
            }

            if (!grpc && config.Communication.UseHttps)
            {
                domain = domain.Replace("http", "https");
                httpClientName = HttpClientNames.RetryHttps;
            }

            var client = _httpClientFactory.CreateClient(httpClientName);
            client.BaseAddress = new(domain);
            if (config.Communication.IgnoreTimeOut)
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
            }

            if (config.Communication.UseIdentityServer4Valid)
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", await _token.GetToken(config));
            };

            return client.As<T>()!;
        }
    }
}
