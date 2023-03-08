using Ars.Commom.Tool.Extension;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.EFCore.EfCoreUnitOfWorks;
using Ars.Common.EFCore.Extension;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.EFCore.AdoNet
{
    internal class DbWriter<TDbContext> : IDbWriter<TDbContext>
        where TDbContext : DbContext
    {
        [Autowired]
        public IDbContextResolver DbContextResolver { get; set; }

        protected TDbContext DbContext => DbContextResolver.Resolve<TDbContext>();

        protected SqlConnection SqlConnection => (SqlConnection)DbContext.Database.GetDbConnection();

        public Task<IDbContextTransaction> BeginAsync(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.RepeatableRead) 
        {
            return DbContextResolver.Resolve<TDbContext>().Database.BeginTransactionAsync(isolationLevel);
        }

        public void Dispose()
        {

        }

        public async Task<int> ExecuteNonQuery(string commandText, SqlParameter[]? parameters, IDbContextTransaction? dbTransaction = null)
        {
            if(SqlConnection.State == ConnectionState.Closed)
                SqlConnection.Open();

            using var command = SqlConnection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = dbTransaction?.GetDbTransaction().As<SqlTransaction>();

            if (parameters?.Any() ?? false)
                command.Parameters.AddRange(parameters);

            return await command.ExecuteNonQueryAsync();
        }
    }
}
