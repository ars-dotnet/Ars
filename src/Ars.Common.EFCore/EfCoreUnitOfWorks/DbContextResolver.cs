using Ars.Commom.Tool.Extension;
using Autofac.Core.Lifetime;
using Autofac.Extensions.DependencyInjection;
using MathNet.Numerics.Providers.Common.Mkl;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public DbContextResolver(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public TDbContext Resolve<TDbContext>(string? connectString = null, DbConnection? existDbConnection = null)
            where TDbContext : DbContext
        {
            TDbContext dbContext;
            var provider = _serviceScopeFactory.CreateScope().ServiceProvider;

            if (connectString.IsNotNullOrEmpty() && null != existDbConnection)
            {
                var storage = provider.GetRequiredService<IEFCoreExistTransactionConnectionStorage>();
                storage.AddConnection(connectString!, existDbConnection);
            }

            dbContext = provider.GetRequiredService<TDbContext>();

            return dbContext;
        }
    }
}
