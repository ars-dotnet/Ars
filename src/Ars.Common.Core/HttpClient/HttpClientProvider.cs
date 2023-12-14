using Ars.Common.Tool.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core
{
    internal class HttpClientProvider : IHttpClientProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClientProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public HttpClient CreateClient(bool isHttps)
        {
            string clientName = isHttps ? HttpClientNames.Https : HttpClientNames.Http;
            
            return _httpClientFactory.CreateClient(clientName);
        }

        public HttpClient CreateClient(string clientName)
        {
            return _httpClientFactory.CreateClient(clientName);
        }
    }
}
