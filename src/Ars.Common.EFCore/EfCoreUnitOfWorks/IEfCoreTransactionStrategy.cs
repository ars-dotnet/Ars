using Ars.Common.Core.Uow.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWorks
{
    public interface IEfCoreTransactionStrategy
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="options"></param>
        void InitOptions(UnitOfWorkOptions options);

        //获取存活的DbContext
        Task<TDbContext> CreateDbContextAsync<TDbContext>(string connectionString, IDbContextResolver dbContextResolver)
            where TDbContext : DbContext;

        //获取存活的DbContext
        TDbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver)
            where TDbContext : DbContext;

        //提交事务
        Task CommitAsync();

        void Dispose();

        IDbContextTransaction? GetContextTransaction(string name = null);
    }
}
