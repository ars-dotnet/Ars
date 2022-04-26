using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    internal class ArsSession : IArsSession, IScopedDependency
    {
        private readonly IPrincipalAccessor _principalAccessor;
        public ArsSession(IPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }

        public int? UserId
        {
            get => int.TryParse(_principalAccessor.claimsPrincipal?.Claims?.FirstOrDefault(r => r.Type == ArsClaimTypes.UserId)?.Value, out int userid) ? userid : null;
            set => UserId = value; 
        }
        public int? TenantId 
        { 
            get => int.TryParse(_principalAccessor.claimsPrincipal?.Claims?.FirstOrDefault(r => r.Type == ArsClaimTypes.TenantId)?.Value, out int tenantid) ? tenantid : null;
            set => TenantId = value;
        }
    }
}
