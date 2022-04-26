using Ars.Common.Core.IDependency;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    internal class AspNetCorePrincipalAccessor : IPrincipalAccessor,ISingletonDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AspNetCorePrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? claimsPrincipal => _httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}
