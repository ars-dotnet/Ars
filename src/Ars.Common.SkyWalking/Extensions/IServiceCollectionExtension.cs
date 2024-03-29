﻿using Ars.Commom.Core;
using Ars.Common.SkyWalking.Diagnostics.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkyApm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SkyWalking.Extensions
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IArsWebApplicationBuilder AddArsSkyApm(this IArsWebApplicationBuilder builder) 
        {
            var services = builder.Services;
            //using var scope = services.BuildServiceProvider().CreateScope();
            //IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            //var skyapmconfig = configuration
            //    .GetSection(nameof(ArsSkyWalkingConfiguration))
            //    .Get<ArsSkyWalkingConfiguration>()
            //    ?? 
            //    throw new ArgumentNullException("appsettings => ArsSkyWalkingConfiguration not be null");

            //必须放在var builder = WebApplication.CreateBuilder(args)之前才生效
            //Environment.SetEnvironmentVariable(nameof(skyapmconfig.ASPNETCORE_HOSTINGSTARTUPASSEMBLIES), skyapmconfig.ASPNETCORE_HOSTINGSTARTUPASSEMBLIES);
            //Environment.SetEnvironmentVariable(nameof(skyapmconfig.SKYWALKING__SERVICENAME), skyapmconfig.SKYWALKING__SERVICENAME);

            services.AddSingleton<ITracingDiagnosticProcessor, ArsEntityFrameworkCoreTracingDiagnosticProcessor>();
            services.AddSkyAPM();

            return builder;
        }
    }
}
