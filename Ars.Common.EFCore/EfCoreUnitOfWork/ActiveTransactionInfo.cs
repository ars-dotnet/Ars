using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWork
{
    internal class ActiveTransactionInfo
    {
        public IDbContextTransaction dbContextTransaction { get; }

        public List<DbContext> AttendedDbContexts { get; }

        public ActiveTransactionInfo(IDbContextTransaction dbContextTransaction)
        {
            this.dbContextTransaction = dbContextTransaction;
            this.AttendedDbContexts = new List<DbContext>();
        }

        public void AddDbContext(DbContext dbContext) 
        {
            AttendedDbContexts.Add(dbContext);
        }
    }
}
