using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.EFCore.Extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore
{
    internal class ArsEFCoreServiceExtension<TDbContext> : IArsServiceExtension
        where TDbContext : ArsDbContext
    {
        private readonly Action<DbContextOptionsBuilder>? _optAction;
        public ArsEFCoreServiceExtension(Action<DbContextOptionsBuilder>? optAction)
        {
            _optAction = optAction;
        }

        public void AddService(IArsWebApplicationBuilder applicationBuilder)
        {
            applicationBuilder.AddArsDbContext<TDbContext>(_optAction);
        }
    }
}
