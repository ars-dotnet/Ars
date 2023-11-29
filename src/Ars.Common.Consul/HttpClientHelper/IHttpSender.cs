using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.HttpClientHelper
{
    public interface IHttpSender
    {
        Task<string> GetAsync(HttpClient httpClient, string? requestUri);

        Task<T?> GetAsync<T>(HttpClient httpClient, string? requestUri);

        Task<string> PostAsync(HttpClient httpClient, string? requestUri, HttpContent? content);
    }
}
