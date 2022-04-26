using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    public interface IArsSession
    {
        int? UserId { get; set; }

        int? TenantId { get; set; }
    }
}
