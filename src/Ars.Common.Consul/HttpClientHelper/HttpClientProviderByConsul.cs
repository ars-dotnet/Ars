using Ars.Commom.Tool.Extension;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Tool;
using Ars.Common.Tool.Tools;
using Microsoft.AspNetCore.Http;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.HttpClientHelper
{
    internal class HttpClientProviderByConsul : IHttpClientProviderByConsul, ISingletonDependency
    {
        private readonly ConsulHelper _consulHelper;
        private readonly IConsulDiscoverConfiguration _options;
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly IToken _token;
        public HttpClientProviderByConsul(
            ConsulHelper consulHelper,
            IConsulDiscoverConfiguration option,
            IHttpClientProvider httpClientProvider,
            IToken token)
        {
            _consulHelper = consulHelper;
            _options = option;
            _httpClientProvider = httpClientProvider;
            _token = token;
        }

        public Task<T> GetHttpClientAsync<T>(string serviceName, string httpClientName = "") where T : HttpClient
        {
            var option = _options.ConsulDiscovers.
                   FirstOrDefault(r => r.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArsException($"consul service:{serviceName} not find");

            return GetHttpClientAsync<T>(option, httpClientName);
        }

        public Task<T> GetHttpClientAsync<T>(ConsulConfiguration config, string httpClientName = "") where T : HttpClient
        {
            if (httpClientName.IsNullOrEmpty())
                httpClientName = HttpClientNames.RetryHttp;

            return GetClient<T>(config, httpClientName);
        }

        public Task<T> GetGrpcHttpClientAsync<T>(string serviceName) where T : HttpClient 
        {
            var option = _options.ConsulDiscovers.
                   FirstOrDefault(r => r.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArsException($"consul service:{serviceName} not find");

            return GetGrpcHttpClientAsync<T>(option);
        }

        public Task<T> GetGrpcHttpClientAsync<T>(ConsulConfiguration config) where T : HttpClient
        {
            return GetClient<T>(config, HttpClientNames.RetryHttp, grpc: true);
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
                        httpClientName = HttpClientNames.RetryGrpcHttpsV1;
                    }
                    else
                    {
                        httpClientName = HttpClientNames.RetryGrpcHttpV1;
                    }
                }
                else if (config.Communication.UseHttps)
                {
                    httpClientName = HttpClientNames.RetryHttps;
                }
            }

            if (!grpc && config.Communication.UseHttps)
            {
                httpClientName = HttpClientNames.RetryHttps;
            }

            var client = _httpClientProvider.CreateClient(httpClientName);
            client.BaseAddress = new(domain);
            if (config.Communication.IgnoreTimeOut)
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
            }

            //非grpc请求时，如有身份认证，则请求获取token
            if (config.Communication.UseIdentityServer4Valid && !grpc)
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", await _token.GetToken(config));
            };

            return client.As<T>()!;
        }
    }
}
