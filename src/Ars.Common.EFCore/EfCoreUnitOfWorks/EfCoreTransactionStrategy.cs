using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow.Options;
using Ars.Common.EFCore.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.EFCore.EfCoreUnitOfWorks
{
    internal class EfCoreTransactionStrategy : IEfCoreTransactionStrategy,ITransientDependency
    {
        private UnitOfWorkOptions _options;
        protected ConcurrentDictionary<string, ActiveTransactionInfo> ActiveTransactionInfos { get; }

        public EfCoreTransactionStrategy()
        {
            ActiveTransactionInfos = new ConcurrentDictionary<string, ActiveTransactionInfo>(
                StringComparer.OrdinalIgnoreCase);
        }

        public void InitOptions(UnitOfWorkOptions options)
        {
            _options = options;
        }

        
        public async Task<TDbContext> CreateDbContextAsync<TDbContext>(string connectionString, IDbContextResolver dbContextResolver) where TDbContext : DbContext
        {
            TDbContext dbContext;

            var activeinfo = ActiveTransactionInfos.GetValueOrDefault(connectionString);
            if (null == activeinfo)
            {
                dbContext = dbContextResolver.Resolve<TDbContext>();

                //默认采用可重复读事务隔离级别
                var dbTransaction = await dbContext.Database.BeginTransactionAsync(
                        (_options.IsolationLevel ?? IsolationLevel.RepeatableRead).ToSystemDataIsolationLevel());

                activeinfo = new ActiveTransactionInfo(dbTransaction, dbContext);
                ActiveTransactionInfos[connectionString] = activeinfo;
            }
            else 
            {
                dbContext = dbContextResolver.Resolve<TDbContext>(
                    connectionString,
                    activeinfo.DbContextTransaction.GetDbTransaction().Connection);

                if (dbContext.HasRelationalTransactionManager()) //关系型数据库使用共享事务
                {
                    await dbContext.Database.UseTransactionAsync(activeinfo.DbContextTransaction.GetDbTransaction());
                }
                else
                {
                    await dbContext.Database.BeginTransactionAsync();
                }

                activeinfo.AttendedDbContexts.Add(dbContext);
            }

            return dbContext;
        }

        public TDbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver) where TDbContext : DbContext
        {
            TDbContext dbContext;

            var activeinfo = ActiveTransactionInfos.GetValueOrDefault(connectionString);
            if (null == activeinfo)
            {
                dbContext = dbContextResolver.Resolve<TDbContext>();

                //默认采用可重复读事务隔离级别
                var dbTransaction = dbContext.Database.BeginTransaction(
                        (_options.IsolationLevel ?? IsolationLevel.RepeatableRead).ToSystemDataIsolationLevel());

                activeinfo = new ActiveTransactionInfo(dbTransaction, dbContext);
                ActiveTransactionInfos[connectionString] = activeinfo;
            }
            else
            {
                dbContext = dbContextResolver.Resolve<TDbContext>(
                    connectionString,
                    activeinfo.DbContextTransaction.GetDbTransaction().Connection);

                if (dbContext.HasRelationalTransactionManager())
                {
                    dbContext.Database.UseTransaction(activeinfo.DbContextTransaction.GetDbTransaction());
                }
                else
                {
                    dbContext.Database.BeginTransaction();
                }

                activeinfo.AttendedDbContexts.Add(dbContext);
            }

            return dbContext;
        }

        public async Task CommitAsync()
        {
            foreach (var activeTrans in ActiveTransactionInfos.Values.ToImmutableList())
            {
                await activeTrans.DbContextTransaction.CommitAsync();

                foreach (var dbcontext in activeTrans.AttendedDbContexts)
                {
                    if (dbcontext.HasRelationalTransactionManager())
                    {
                        continue;
                    }

                    await dbcontext.Database.CommitTransactionAsync();
                }
            }
        }


        public void Dispose()
        {
            foreach (var activeTransaction in ActiveTransactionInfos.Values)
            {
                activeTransaction.DbContextTransaction.Dispose();
                activeTransaction.DbContext?.Dispose();
                foreach (var db in activeTransaction.AttendedDbContexts) 
                {
                    db.Dispose();
                }

                activeTransaction.AttendedDbContexts.Clear();
            }

            ActiveTransactionInfos.Clear();
        }

        public IDbContextTransaction? GetContextTransaction(string name)
        {
            return ActiveTransactionInfos.TryGetValue(name, out var value) ? value.DbContextTransaction : null;
        }

        public DbConnection? GetContextTransactionConnection(string name)
        {
            return GetContextTransaction(name)?.GetDbTransaction()?.Connection;
        }

    }
}
