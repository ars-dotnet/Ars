using Ars.Commom.Core;
using Ars.Commom.Tool.Serializer;
using Ars.Common.AutoFac.Dependency;
using Ars.Common.AutoFac.Extension;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Ars.Common.AutoFac.RegisterProvider;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Extension;
using Ars.Common.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Host.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArserviceCore(
            this IServiceCollection services,
            IHostBuilder host)
        {
            var arsbuilder = new ArsServiceBuilder(new ArsServiceCollection(services), host);
            arsbuilder.AddArsAspNetCore();
            arsbuilder.AddArsAutofac();
          
            services.AddSingleton<IArsSerializer, ArsSerializer>();
            services.AddSingleton<IArsConfiguration>(new ArsConfiguration());
            
            return arsbuilder;
        }
    }
}
