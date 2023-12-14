using Ars.Commom.Tool.Certificates;
using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.Extension
{
    public static class IHttpClientBuilderExtension
    {
        /// <summary>
        /// 配置grpc https MessageHandler
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureArsGrpcPrimaryHttpsMessageHandler(
            this IHttpClientBuilder builder, 
            Func<HttpMessageHandler, HttpMessageHandler> func) 
        {
            return builder.ConfigureArsPrimaryHttpsMessageHandler(func);
        }
    }
}
