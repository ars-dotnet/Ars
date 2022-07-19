using Ars.Common.Core.IDependency;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWorks
{
    public interface IDbContextResolver : ITransientDependency
    {
        TDbContext Resolve<TDbContext>(string connectionstring = null, DbConnection dbConnection = null)
            where TDbContext : DbContext;
    }
}
