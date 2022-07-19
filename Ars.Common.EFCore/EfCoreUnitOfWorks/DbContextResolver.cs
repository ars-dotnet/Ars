using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWorks
{
    internal class DbContextResolver : IDbContextResolver
    {
        private readonly IServiceProvider _serviceProvider;
        public DbContextResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TDbContext Resolve<TDbContext>(string connectionstring, DbConnection dbConnection) where TDbContext : DbContext
        {
            return _serviceProvider.GetRequiredService<TDbContext>();
        }
    }
}
