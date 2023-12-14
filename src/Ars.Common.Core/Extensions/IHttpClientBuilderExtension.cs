using Ars.Commom.Tool.Certificates;
using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Ars.Common.Tool.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Extensions
{
    public static class IHttpClientBuilderExtension
    {
        /// <summary>
        /// 配置https MessageHandler
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureArsPrimaryHttpsMessageHandler(
            this IHttpClientBuilder builder,
            Func<HttpMessageHandler, HttpMessageHandler>? func = null) 
        {
            return builder.ConfigurePrimaryHttpMessageHandler(e =>
            {
                var handler = new HttpClientHandler();
                handler.SslProtocols = SslProtocols.Tls12;
                handler.AllowAutoRedirect = true;
                handler.UseCookies = true;
                handler.CookieContainer = new CookieContainer();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                var option = e.GetRequiredService<IOptions<IArsBasicConfiguration>>().Value;
                if (null != option &&
                    option.CertificatePath.IsNotNullOrEmpty() &&
                    option.CertificatePassWord.IsNotNullOrEmpty())
                {
                    handler.ClientCertificates.Add(
                        Certificate.Get(option.CertificatePath!, option.CertificatePassWord!));
                }

                handler.ClientCertificateOptions = ClientCertificateOption.Automatic;

                HttpMessageHandler httpMessageHandler = 
                    null == func 
                    ? handler 
                    : func(handler);

                return httpMessageHandler;
            });
        }

        /// <summary>
        /// 添加降级 熔断 超时 重试策略
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddArsTransientHttpErrorPolicy(this IHttpClientBuilder builder) 
        {
            return builder.AddTransientHttpErrorPolicy(policyBuilder =>
            {
                return policyBuilder.AddArsHttpClientPolicy();
            });
        }
    }
}
