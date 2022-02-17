using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Extension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsIdentityServer4(this IApplicationBuilder builder, IdentityServerMiddlewareOptions options = null) 
        {
            builder.UseIdentityServer(options);
            return builder;
        }
    }
}
