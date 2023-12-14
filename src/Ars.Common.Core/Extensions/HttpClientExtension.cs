using Ars.Commom.Core;
using Ars.Commom.Tool.Certificates;
using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Extensions
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// add http client services
        /// </summary>
        /// <param name="arsServiceBuilder"></param>
        /// <returns></returns>
        public static IArsWebApplicationBuilder AddArsHttpClient(this IArsWebApplicationBuilder arsServiceBuilder)
        {
            var services = arsServiceBuilder.Services;

            services.AddHttpClient(HttpClientNames.Http);

            services
                .AddHttpClient(HttpClientNames.Https)
                .ConfigureArsPrimaryHttpsMessageHandler();

            #region 降级 熔断 超时 重试 
            services
                .AddHttpClient(HttpClientNames.RetryHttp)
                .AddArsTransientHttpErrorPolicy();

            services
                .AddHttpClient(HttpClientNames.RetryHttps)
                .ConfigureArsPrimaryHttpsMessageHandler()
                .AddArsTransientHttpErrorPolicy();
            #endregion

            return arsServiceBuilder;
        }

    }
}
