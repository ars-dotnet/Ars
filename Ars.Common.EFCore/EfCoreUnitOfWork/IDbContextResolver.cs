using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWork
{
    public interface IDbContextResolver
    {
        TDbContext Resolve<TDbContext>(string connectionstring, DbConnection dbConnection)
            where TDbContext : DbContext;
    }
}
