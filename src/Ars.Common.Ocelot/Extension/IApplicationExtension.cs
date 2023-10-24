using Microsoft.AspNetCore.Builder;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot.Extension
{
    public static class IApplicationExtension
    {
        public static IApplicationBuilder UseArsOcelot(this IApplicationBuilder builder) 
        {
            //use swagger for ocelot
            builder
            .UseSwaggerForOcelotUI(opt =>
            {
                opt.DownstreamSwaggerEndPointBasePath = "/swagger/docs";
                opt.PathToSwaggerGenerator = "/swagger/docs";
            })
            .UseWebSockets() 
            .UseOcelot().Wait();

            return builder;
        }
    }
}
