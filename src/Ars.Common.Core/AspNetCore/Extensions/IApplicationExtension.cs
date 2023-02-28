using Ars.Common.Core.AspNetCore.MiddleWare;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.Extensions
{
    public static class IApplicationExtension
    {
        public static IApplicationBuilder UsArsExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware(typeof(ArsExceptionMiddleWare));
        }
    }
}
