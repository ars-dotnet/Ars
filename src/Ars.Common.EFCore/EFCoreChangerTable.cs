using Ars.Common.Core.Diagnostic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore
{
    internal class EFCoreChangerTable : ChangerTable
    {
        public EntityEntry EntityEntry { get; set; }
    }
}
