using Ars.Commom.Core;
using Aspose.Cells;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;
using Ars.Common.Core.Extensions;
using Ars.Common.RpcClientCore.Options;

namespace Ars.Common.RpcClientCore.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IHttpClientBuilder AddArsHttpApi<THttpApi>(
            this IArsWebApplicationBuilder arsWebApplicationBuilder,
            Action<ArsHttpApiOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null)
            where THttpApi : class
        {
            var services = arsWebApplicationBuilder.Services;

            var builder = services.AddHttpApi<THttpApi>();
            if (null != configureHttpClientBuilder) 
            {
                configureHttpClientBuilder(builder);
            }

            if (null != configureOptions)
            {
                services
                    .AddOptions<ArsHttpApiOptions>(builder.Name)
                    .Configure(configureOptions);
            }

            services.AddArsHttpApiProvider<THttpApi>(builder.Name);

            //arsWebApplicationBuilder.AddArsHttpClient(configureHttpClientBuilder);

            return builder;
        }

        public static IHttpClientBuilder AddArsHttpApi(
            this IArsWebApplicationBuilder arsWebApplicationBuilder,
            Type httpApiType,
            Action<ArsHttpApiOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null)
        {
            return arsWebApplicationBuilder.AddArsHttpApiInner(httpApiType, configureOptions, configureHttpClientBuilder);
        }

        private static void AddArsHttpApiProvider<THttpApi>(this IServiceCollection services,string httpApiName) where THttpApi : class 
        {
            services.TryAddSingleton(typeof(ArsHttpApiProvider<>));

            services.Replace(new ServiceDescriptor(typeof(THttpApi),
                (sp) => 
                {
                    var httpApiProvider = sp.GetRequiredService<ArsHttpApiProvider<THttpApi>>();
                    return httpApiProvider.CreateHttpApi(sp, httpApiName);
                },
                ServiceLifetime.Transient));
        }

        private static IHttpClientBuilder AddArsHttpApiInner(
            this IArsWebApplicationBuilder services, 
            Type httpApiType,
            Action<ArsHttpApiOptions>? configureOptions = null, 
            Action<IHttpClientBuilder>? configureHttpClientBuilder = null) 
        {
            if (httpApiType == null)
            {
                throw new ArgumentNullException(nameof(httpApiType));
            }

            if (httpApiType.IsGenericTypeDefinition == true)
            {
                throw new NotSupportedException($"{httpApiType.Name}不能是泛型对象");
            }

            return ArsHttpApiAdder.Create(httpApiType).AddHttpApi(services, configureOptions, configureHttpClientBuilder);
        }
    }
}
