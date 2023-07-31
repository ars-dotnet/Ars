using Ars.Commom.Core;
using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Cap.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsCap(this IArsServiceBuilder builder, Action<CapOptions> setupAction)
        {
            var services = builder.Services;

            services.AddTransient<IArsCapPublisher, ArsCapPublisher>();
            services.AddCap(setupAction);

            return builder;
        }
    }
}
