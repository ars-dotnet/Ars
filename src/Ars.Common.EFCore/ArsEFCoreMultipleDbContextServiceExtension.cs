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
    internal class ArsEFCoreMultipleDbContextServiceExtension<TDbContext> : IArsServiceExtension
        where TDbContext : ArsDbContext
    {
        private readonly Action<IServiceProvider, DbContextOptionsBuilder>? _optAction;
        public ArsEFCoreMultipleDbContextServiceExtension(Action<IServiceProvider, DbContextOptionsBuilder>? optAction)
        {
            _optAction = optAction;
        }

        public void AddService(IArsWebApplicationBuilder applicationBuilder)
        {
            applicationBuilder.AddArsMultipleDbContext<TDbContext>(_optAction);
        }
    }
}
