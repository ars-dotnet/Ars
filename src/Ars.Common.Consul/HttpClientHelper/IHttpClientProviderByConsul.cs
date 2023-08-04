using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.HttpClientHelper
{
    /// <summary>
    /// 创建httpClient
    /// BaseAddress从consul中获取
    /// </summary>
    public interface IHttpClientProviderByConsul
    {
        Task<T> GetHttpClientAsync<T>(string serviceName) where T : HttpClient;

        Task<T> GetHttpClientAsync<T>(ConsulConfiguration config) where T : HttpClient;

        Task<T> GetGrpcHttpClientAsync<T>(ConsulConfiguration config) where T : HttpClient;
    }
}
