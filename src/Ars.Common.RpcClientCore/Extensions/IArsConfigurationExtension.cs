using Ars.Common.Core.Configs;
using Ars.Common.RpcClientCore.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;

namespace Ars.Common.RpcClientCore.Extensions
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsHttpApi<THttpApi>(
            this IArsConfiguration arsConfiguration, 
            Action<ArsHttpApiOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureBuilder = null)
            where THttpApi : class
        {
            return arsConfiguration.AddArsServiceExtension(
                new ArsHttpApiServiceExtension<THttpApi>(configureOptions, configureBuilder));
        }

        public static IArsConfiguration AddArsHttpApi(
            this IArsConfiguration arsConfiguration,
            Type httpApiType,
            Action<ArsHttpApiOptions>? configureOptions = null,
            Action<IHttpClientBuilder>? configureBuilder = null)
        {
            return arsConfiguration.AddArsServiceExtension(
                new ArsHttpApiServiceExtension(httpApiType,configureOptions, configureBuilder));
        }
    }
}
