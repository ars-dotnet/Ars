using Ars.Common.Core.Uow;
using Ars.Common.EFCore.EfCoreUnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Extension
{
    public static class UnitOfWorkExtensions
    {
        public static Task<TDbContext> GetDbContextAsync<TDbContext>(this IActiveUnitOfWork activeUnitOfWork, string name = null)
            where TDbContext : DbContext
        {
            if (null == activeUnitOfWork) 
            {
                throw new ArgumentNullException(nameof(activeUnitOfWork));
            }
            if (!(activeUnitOfWork is EfCoreUnitOfWork coreUnitOfWork)) 
            {
                throw new ArgumentException($"{nameof(activeUnitOfWork)} is not type of {typeof(EfCoreUnitOfWork).FullName}",nameof(activeUnitOfWork));
            }

           return coreUnitOfWork.GetOrCreateDbContextAsync<TDbContext>(name);
        }

        public static TDbContext GetDbContext<TDbContext>(this IActiveUnitOfWork activeUnitOfWork, string name = null)
            where TDbContext : DbContext
        {
            if (null == activeUnitOfWork)
            {
                throw new ArgumentNullException(nameof(activeUnitOfWork));
            }
            if (!(activeUnitOfWork is EfCoreUnitOfWork coreUnitOfWork))
            {
                throw new ArgumentException($"{nameof(activeUnitOfWork)} is not type of {typeof(EfCoreUnitOfWork).FullName}", nameof(activeUnitOfWork));
            }

            return coreUnitOfWork.GetOrCreateDbContext<TDbContext>(name);
        }
    }
}
