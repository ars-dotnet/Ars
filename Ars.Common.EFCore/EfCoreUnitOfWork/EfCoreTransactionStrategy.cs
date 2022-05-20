using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWork
{
    internal class EfCoreTransactionStrategy : IEfCoreTransactionStrategy,ITransientDependency
    {
        private UnitOfWorkOptions _options;

        private ConcurrentDictionary<string,ActiveTransactionInfo> _activeTransactionInfos;

        [Autowired]
        public IDbContextResolver dbContextResolver { get; set; }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public async Task<TDbContext> CreateDbContextAsync<TDbContext>(string connectionString) where TDbContext : DbContext
        {
            TDbContext t = null;

            var activeinfo = _activeTransactionInfos.GetValueOrDefault(connectionString);
            if (null == activeinfo) 
            {
                t = dbContextResolver.Resolve<TDbContext>(connectionString,null);
            }

            return t;
        }

        public void InitOptions(UnitOfWorkOptions options)
        {
            _options = options;
        }
    }
}
