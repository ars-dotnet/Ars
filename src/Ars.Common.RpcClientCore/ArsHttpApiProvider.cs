using Ars.Common.Core;
using Ars.Common.RpcClientCore.Options;
using Ars.Common.Tool.Tools;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;

namespace Ars.Common.RpcClientCore
{
    public class ArsHttpApiProvider<THttpApi>
    {
        private readonly IHttpClientProvider httpClientProvider;
        private readonly IOptionsMonitor<ArsHttpApiOptions> httpApiOptionsMonitor;
        private readonly IHttpApiActivator<THttpApi> httpApiActivator;

        /// <summary>
        /// THttpApi提供者
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="httpApiOptionsMonitor"></param>
        /// <param name="httpApiActivator"></param>
        public ArsHttpApiProvider(
            IHttpClientProvider httpClientProvider, 
            IOptionsMonitor<ArsHttpApiOptions> httpApiOptionsMonitor, 
            IHttpApiActivator<THttpApi> httpApiActivator)
        {
            this.httpClientProvider = httpClientProvider;
            this.httpApiOptionsMonitor = httpApiOptionsMonitor;
            this.httpApiActivator = httpApiActivator;
        }

        /// <summary>
        /// 创建THttpApi的实例
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public THttpApi CreateHttpApi(IServiceProvider serviceProvider, string name)
        {
            var httpApiOptions = this.httpApiOptionsMonitor.Get(name);

            var httpClient = this.httpClientProvider.CreateClient(GetClientName(name, httpApiOptions));

            var httpClientContext = new HttpClientContext(httpClient, serviceProvider, httpApiOptions, name);

            var httpApiInterceptor = new ArsHttpApiInterceptor(httpClientContext);

            return this.httpApiActivator.CreateInstance(httpApiInterceptor);
        }

        private string GetClientName(string defaultName, ArsHttpApiOptions arsHttpApiOptions) 
        {
            if(!arsHttpApiOptions.UseArsHttpClient)
                return defaultName;

            if (arsHttpApiOptions.UseHttps)
            {
                return 
                    arsHttpApiOptions.UseHttpClientCustomPolicy 
                    ? HttpClientNames.RetryHttps 
                    : HttpClientNames.Https;
            }
            else 
            {
                return
                    arsHttpApiOptions.UseHttpClientCustomPolicy
                    ? HttpClientNames.RetryHttp
                    : HttpClientNames.Http;
            }
        }
    }
}
