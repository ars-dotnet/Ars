using Ars.Commom.Core;
using Ars.Common.Tool.Extension;
using Ars.Common.Tool.Tools;
using Microsoft.Extensions.DependencyInjection;
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
                .ConfigurePrimaryHttpMessageHandler((e) =>
                {
                    var handler = new HttpClientHandler();
                    handler.AllowAutoRedirect = true;
                    handler.UseCookies = true;
                    handler.CookieContainer = new CookieContainer();
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return handler;
                });

            //超时时间10s
            //HttpRequestException、TimeoutRejectedException、TimeoutException、
            //HttpStatusCode >= 500 || HttpStatusCode == 408[请求超时] 都会触发熔断异常请求计数
            //熔断异常请求达到30个时，服务熔断5s
            //第一次请求失败后，此后有2次重试请求，分别为第1s和第2s
            #region 熔断 超时 重试 
            services
                .AddHttpClient(HttpClientNames.RetryHttp)
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.AddArsHttpClientPolicy();
                });

            services
                .AddHttpClient(HttpClientNames.RetryHttps)
                .ConfigurePrimaryHttpMessageHandler((e) =>
                {
                    var handler = new HttpClientHandler();
                    handler.AllowAutoRedirect = true;
                    handler.UseCookies = true;
                    handler.CookieContainer = new CookieContainer();
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return handler;
                })
                .AddTransientHttpErrorPolicy(policyBuilder =>
                {
                    return policyBuilder.AddArsHttpClientPolicy();
                });
            #endregion

            return arsServiceBuilder;
        }

    }
}
