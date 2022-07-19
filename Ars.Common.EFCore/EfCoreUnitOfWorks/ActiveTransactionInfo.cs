using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWorks
{
    internal class ActiveTransactionInfo
    {
        public IDbContextTransaction DbContextTransaction { get; }

        public DbContext DbContext { get; }

        public List<DbContext> AttendedDbContexts { get; }

        public ActiveTransactionInfo(IDbContextTransaction dbContextTransaction, DbContext dbContext)
        {
            this.DbContextTransaction = dbContextTransaction;
            this.DbContext = dbContext;
            this.AttendedDbContexts = new List<DbContext>();
        }
    }
}
