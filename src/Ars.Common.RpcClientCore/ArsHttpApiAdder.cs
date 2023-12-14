using Ars.Commom.Core;
using Ars.Common.RpcClientCore.Extensions;
using Ars.Common.RpcClientCore.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Exceptions;

namespace Ars.Common.RpcClientCore
{
    public abstract class ArsHttpApiAdder
    {
        /// <summary>
        /// 添加HttpApi代理类到服务
        /// </summary> 
        /// <returns></returns>
        public abstract IHttpClientBuilder AddHttpApi(
            IArsWebApplicationBuilder services, 
            Action<ArsHttpApiOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null);

        /// <summary>
        /// 创建指定接口的HttpApiAdder
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static ArsHttpApiAdder Create(Type httpApiType)
        {
            var adderType = typeof(HttpApiAdderOf<>).MakeGenericType(httpApiType);

            var instance = Activator.CreateInstance(adderType);

            if (instance == null)
            {
                throw new TypeInstanceCreateException(adderType);
            }

            return (ArsHttpApiAdder)instance;
        }

        /// <summary>
        /// 表示HttpApi服务添加者
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        private class HttpApiAdderOf<THttpApi> : ArsHttpApiAdder where THttpApi : class
        {
            /// <summary>
            /// 添加HttpApi代理类到服务
            /// </summary> 
            /// <returns></returns>
            public override IHttpClientBuilder AddHttpApi(
                IArsWebApplicationBuilder builder,
                Action<ArsHttpApiOptions>? configureOptions = null,
                Action<IHttpClientBuilder> ? configureHttpClientBuilder = null)
            {
                return builder.AddArsHttpApi<THttpApi>(configureOptions, configureHttpClientBuilder);
            }
        }
    }
}
