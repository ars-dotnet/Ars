using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.HttpClientHelper
{
    public interface IHttpClientProvider
    {
        Task<T> GetHttpClient<T>(string serviceName) where T : HttpClient;

        Task<T> GetHttpClient<T>(ConsulConfiguration config) where T : HttpClient;

        Task<T> GetGrpcHttpClient<T>(ConsulConfiguration config) where T : HttpClient;
    }
}
