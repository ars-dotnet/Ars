using Ars.Common.EFCore.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWork
{
    public interface IEfCoreTransactionStrategy
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="options"></param>
        void InitOptions(UnitOfWorkOptions options);

        //获取存活的DbContext
        Task<TDbContext> CreateDbContextAsync<TDbContext>(string connectionString)
            where TDbContext : DbContext;

        //提交事务
        void Commit();
    }
}
