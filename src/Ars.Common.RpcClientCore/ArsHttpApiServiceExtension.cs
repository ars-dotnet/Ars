using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.RpcClientCore.Extensions;
using Ars.Common.RpcClientCore.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;

namespace Ars.Common.RpcClientCore
{
    internal class ArsHttpApiServiceExtension<THttpApi> : IArsServiceExtension
        where THttpApi : class
    {
        private readonly Action<ArsHttpApiOptions>? _configureOptions;
        private readonly Action<IHttpClientBuilder>? _configureHttpClientBuilder;
        public ArsHttpApiServiceExtension(
            Action<ArsHttpApiOptions>? configureOptions,
            Action<IHttpClientBuilder>? configureHttpClientBuilder)
        {
            _configureOptions = configureOptions;
            _configureHttpClientBuilder = configureHttpClientBuilder;
        }

        public void AddService(IArsWebApplicationBuilder applicationBuilder)
        {
            applicationBuilder.AddArsHttpApi<THttpApi>(_configureOptions, _configureHttpClientBuilder);
        }
    }

    internal class ArsHttpApiServiceExtension : IArsServiceExtension
    {
        private readonly Type _httpApiType;
        private readonly Action<ArsHttpApiOptions>? _configureOptions;
        private readonly Action<IHttpClientBuilder>? _configureHttpClientBuilder;
        public ArsHttpApiServiceExtension(
            Type httpApiType,
            Action<ArsHttpApiOptions>? configureOptions,
            Action<IHttpClientBuilder>? configureHttpClientBuilder)
        {
            _httpApiType = httpApiType;
            _configureOptions = configureOptions;
            _configureHttpClientBuilder = configureHttpClientBuilder;
        }

        public void AddService(IArsWebApplicationBuilder applicationBuilder)
        {
            applicationBuilder.AddArsHttpApi(_httpApiType, _configureOptions, _configureHttpClientBuilder);
        }
    }
}
