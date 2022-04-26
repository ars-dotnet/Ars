using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal? claimsPrincipal { get; }
    }
}
