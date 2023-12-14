using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Extension
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsDbContext<TDbContext>(
            this IArsConfiguration arsConfiguration,
            Action<DbContextOptionsBuilder>? optAction = null)
            where TDbContext : ArsDbContext
        {
            return arsConfiguration.AddArsServiceExtension(
                new ArsEFCoreServiceExtension<TDbContext>(optAction));
        }

        public static IArsConfiguration AddArsMultipleDbContext<TDbContext>(
            this IArsConfiguration arsConfiguration,
           Action<IServiceProvider, DbContextOptionsBuilder>? optAction = null)
            where TDbContext : ArsDbContext
        {
            return arsConfiguration.AddArsServiceExtension(
                new ArsEFCoreMultipleDbContextServiceExtension<TDbContext>(optAction));
        }
    }
}
