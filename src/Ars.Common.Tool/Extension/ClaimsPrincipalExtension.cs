using Ars.Commom.Tool.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class ClaimsPrincipalExtension
    {
        public static bool Logined(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(r => r.Type.Equals(ClaimTypes.NameIdentifier))?.Value?.IsNotNullOrEmpty() ?? false;
        }

        public static string? GetValue(this ClaimsPrincipal claimsPrincipal, string key) 
        {
            return claimsPrincipal.Claims.FirstOrDefault(r => r.Type.Equals(key))?.Value;
        }
    }
}
