using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Entities
{
    public interface IMayHaveTenant
    {
        int? TenantId { get; set; }
    }
}
