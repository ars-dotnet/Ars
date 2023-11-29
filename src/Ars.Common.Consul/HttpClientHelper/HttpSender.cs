using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.HttpClientHelper
{
    internal class HttpSender : IHttpSender, ISingletonDependency
    {
        public async Task<string> GetAsync(HttpClient httpClient, string? requestUri)
        {
            using var res = await httpClient.GetAsync(requestUri);
            
            res.EnsureSuccessStatusCode();

            return await res.Content.ReadAsStringAsync();
        }

        public async Task<T?> GetAsync<T>(HttpClient httpClient, string? requestUri)
        {
            using var res = await httpClient.GetAsync(requestUri);

            res.EnsureSuccessStatusCode();

            return await res.Content.ReadFromJsonAsync<T>();
        }

        public async Task<string> PostAsync(HttpClient httpClient, string? requestUri, HttpContent? content)
        {
            using var res = await httpClient.PostAsync(requestUri, content);
            
            res.EnsureSuccessStatusCode();

            return await res.Content.ReadAsStringAsync();
        }
    }
}
