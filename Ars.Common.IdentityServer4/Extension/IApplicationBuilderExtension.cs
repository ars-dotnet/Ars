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
        public static IApplicationBuilder UseArsIdentityServer(this IApplicationBuilder builder) 
        {
            builder.UseIdentityServer();
            return builder;
        }

        public static IApplicationBuilder UseArsIdentityClient(this IApplicationBuilder builder) 
        {
            builder.UseAuthentication();
            builder.UseAuthorization();

            return builder;
        }
    }
}
