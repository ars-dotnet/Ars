using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    public class ArsClaimTypes
    {
        public const string SetUserId = "sub";

        public const string GetUserId = ClaimTypes.NameIdentifier;

        public const string UserName = ClaimTypes.Name;

        public const string Role = ClaimTypes.Role;

        public const string TenantId = "tenant";
    }
}
