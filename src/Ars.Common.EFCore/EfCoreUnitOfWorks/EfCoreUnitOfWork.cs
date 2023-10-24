using Ars.Common.Core.IDependency;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ars.Commom.Tool.Extension;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Collections.Immutable;
using Ars.Common.Core.Uow.Impl;
using System.Configuration;
using Ars.Common.Tool;
using Microsoft.EntityFrameworkCore.Storage;
using Ars.Common.Core.Diagnostic;
using System.Diagnostics;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Uow.Options;

namespace Ars.Common.EFCore.EfCoreUnitOfWorks
{
    public class EfCoreUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        protected static readonly DiagnosticListener s_diagnosticListener =
             new(ArsDiagnosticNames.ListenerName);

        protected IDictionary<string, DbContext> ActiveDbContexts { get; }
        private readonly IDbContextResolver _dbContextResolver;
        private readonly IEfCoreTransactionStrategy _efCoreTransactionStrategy;
        private readonly IArsDbContextConfiguration? _arsDbContextConfiguration;
        private readonly IArsMultipleDbContextConfiguration? _arsMultipleDbContextConfiguration;

        public EfCoreUnitOfWork(IDbContextResolver dbContextResolver,
            IEfCoreTransactionStrategy efCoreTransactionStrategy,
            IArsDbContextConfiguration? arsDbContextConfiguration = null,
            IArsMultipleDbContextConfiguration? arsMultipleDbContextConfiguration = null)
        {
            _dbContextResolver = dbContextResolver;
            _efCoreTransactionStrategy = efCoreTransactionStrategy;
            _arsDbContextConfiguration = arsDbContextConfiguration;
            _arsMultipleDbContextConfiguration = arsMultipleDbContextConfiguration;

            ActiveDbContexts = new Dictionary<string, DbContext>(StringComparer.OrdinalIgnoreCase);
        }

        protected override void BeginUow()
        {
            if (Options.IsTransactional == true)
            {
                _efCoreTransactionStrategy.InitOptions(Options);
            }
        }

        public override async Task SaveChangesAsync()
        {
            foreach (var dbContext in GetActiveDbContexts())
            {
                await SaveChangesInDbContextAsync(dbContext);
            }
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();
            await CommitTransaction();
        }

        protected override void DisposeUow()
        {
            if (Options.IsTransactional == true)
            {
                _efCoreTransactionStrategy.Dispose();
            }
            else
            {
                foreach (var dbContext in GetActiveDbContexts())
                {
                    dbContext.Dispose();
                }
            }

            ActiveDbContexts.Clear();
        }

        protected virtual Task SaveChangesInDbContextAsync(DbContext dbContext)
        {
            return dbContext.SaveChangesAsync();
        }

        public virtual string GetConnectionName<TDbContext>()
        {
            string? defaultstring = 
                _arsMultipleDbContextConfiguration?.ArsDbContextConfiguration?
                    .FirstOrDefault(r => r.DbContextFullName.Equals(typeof(TDbContext).FullName))?.DefaultString
                ?? _arsDbContextConfiguration?.DefaultString;
            if(!defaultstring.IsNullOrEmpty())
                return defaultstring!;

            if (ConfigurationManager.ConnectionStrings["ArsDbContextConfiguration:DefaultString"] != null)
            {
                return "Default";
            }

            if (ConfigurationManager.ConnectionStrings.Count == 1)
            {
                return ConfigurationManager.ConnectionStrings[0].ConnectionString;
            }

            throw new ArsException("Could not find a connection string definition for the application.");
        }

        public virtual async Task<TDbContext> GetOrCreateDbContextAsync<TDbContext>(string name = null)
            where TDbContext : DbContext
        {
            var dbcontextKey = GetKeyName<TDbContext>(out string connectionName, name);
            if (ActiveDbContexts.TryGetValue(dbcontextKey, out var dbContext))//存同一个dbcontext，相同的连接
            {
                return (TDbContext)dbContext;
            }

            if (Options.IsTransactional == true)
            {
                dbContext = await _efCoreTransactionStrategy
                   .CreateDbContextAsync<TDbContext>(connectionName, _dbContextResolver);
            }
            else
            {
                dbContext = _dbContextResolver.Resolve<TDbContext>();
            }

            ActiveDbContexts.TryAdd(dbcontextKey, dbContext);
            return (TDbContext)dbContext;
        }

        public virtual IDbContextTransaction? GetContextTransaction<TDbContext>(string name = null)
             where TDbContext : DbContext
        {
            GetKeyName<TDbContext>(out string connectionName, name);
            return _efCoreTransactionStrategy.GetContextTransaction(connectionName);
        }

        public virtual TDbContext GetOrCreateDbContext<TDbContext>(string name = null)
            where TDbContext : DbContext
        {
            var dbcontextKey = GetKeyName<TDbContext>(out string connectionName,name);

            if (ActiveDbContexts.TryGetValue(dbcontextKey, out var dbContext))
            {
                return (TDbContext)dbContext;
            }

            //新建一个dbcontext
            if (Options.IsTransactional == true)
            {
                dbContext = _efCoreTransactionStrategy
                   .CreateDbContext<TDbContext>(connectionName, _dbContextResolver);
            }
            else
            {
                dbContext = _dbContextResolver.Resolve<TDbContext>();
            }

            ActiveDbContexts.TryAdd(dbcontextKey, dbContext);
            return (TDbContext)dbContext;
        }

        private async Task CommitTransaction() 
        {
            if (Options.IsTransactional == true) 
            {
                await _efCoreTransactionStrategy.CommitAsync();
            }
        }

        private string GetKeyName<TDbContext>(out string connectionName, string name = null)
            where TDbContext : DbContext
        {
            if (typeof(TDbContext).GetTypeInfo().IsAbstract)
            {
                throw new ArgumentException($"{nameof(TDbContext)} not support abstract class");
            }

            connectionName = GetConnectionName<TDbContext>();
            string dbcontextKey = typeof(TDbContext).FullName + connectionName;
            if (!name.IsNullOrEmpty())
                dbcontextKey += "." + name;

            return dbcontextKey;
        }

        public IEnumerable<DbContext> GetActiveDbContexts() 
        {
            return ActiveDbContexts.Values.ToImmutableList();
        }

        protected override void Publish()
        {
            if (s_diagnosticListener.IsEnabled(ArsDiagnosticNames.CompleteTransactionName)) 
            {
                foreach (var dbcontext in GetActiveDbContexts())
                {
                    var changerTables = ((ArsDbContext)dbcontext).GetChangerTables();
                    if(changerTables.HasValue())
                    {
                        ArsCommandEventData arsCommandEventData = new ArsCommandEventData()
                        {
                            DbCommand = dbcontext.Database.GetDbConnection().CreateCommand(),
                            ChangerTables = changerTables!,
                        };

                        s_diagnosticListener.Write(ArsDiagnosticNames.CompleteTransactionName, arsCommandEventData);
                    }
                }
            }
        }
    }
}
