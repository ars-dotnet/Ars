using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.Extensions
{
    public static class HttpContextExtension
    {
        public static string? GetUserId(this HttpContext httpContext) 
        {
            return httpContext?.User?.Claims?.FirstOrDefault(r => r.Type == ArsClaimTypes.UserId)?.Value ?? null;
        }

        public static string? GetUserName(this HttpContext httpContext)
        {
            return httpContext?.User?.Claims?.FirstOrDefault(r => r.Type == ArsClaimTypes.UserName)?.Value ?? null;
        }
    }
}
