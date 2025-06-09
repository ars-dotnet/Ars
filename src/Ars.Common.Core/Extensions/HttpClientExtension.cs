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
        public static IArsWebApplicationBuilder AddArsHttpClient(
            this IArsWebApplicationBuilder arsServiceBuilder,
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null)
        {
            var services = arsServiceBuilder.Services;

            var builder = services.AddHttpClient(HttpClientNames.Http);

            configureHttpClientBuilder?.Invoke(builder);

            builder = services
                .AddHttpClient(HttpClientNames.Https)
                .ConfigureArsPrimaryHttpsMessageHandler();

            configureHttpClientBuilder?.Invoke(builder);

            #region 降级 熔断 超时 重试 
            builder = services
                .AddHttpClient(HttpClientNames.RetryHttp)
                .AddArsTransientHttpErrorPolicy();

            configureHttpClientBuilder?.Invoke(builder);

            builder = services
                .AddHttpClient(HttpClientNames.RetryHttps)
                .ConfigureArsPrimaryHttpsMessageHandler()
                .AddArsTransientHttpErrorPolicy();

            configureHttpClientBuilder?.Invoke(builder);

            #endregion

            return arsServiceBuilder;
        }

    }
}
