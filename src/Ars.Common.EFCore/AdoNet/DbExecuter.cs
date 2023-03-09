using Ars.Commom.Tool.Extension;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.EFCore.EfCoreUnitOfWorks;
using Ars.Common.EFCore.Extension;
using Ars.Common.Tool;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
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
    internal class DbExecuter<TDbContext> : IDbExecuter<TDbContext>
        where TDbContext : DbContext
    {
        public DbExecuter(IDbContextResolver dbContextResolver)
        {
            DbContext = dbContextResolver.Resolve<TDbContext>();
            SqlConnection = (SqlConnection)DbContext.Database.GetDbConnection();
        }

        protected TDbContext DbContext { get; }

        protected SqlConnection SqlConnection { get; }

        protected IDbContextTransaction? DbContextTransaction { get; set; }

        public async Task<IDbContextTransaction> BeginAsync(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.RepeatableRead)
        {
            DbContextTransaction = await DbContext.Database.BeginTransactionAsync(isolationLevel);
            return DbContextTransaction;
        }

        public async Task<int> ExecuteNonQuery(string commandText, SqlParameter[]? parameters)
        {
            if (SqlConnection.State == ConnectionState.Closed)
                SqlConnection.Open();

            using var command = SqlConnection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = DbContextTransaction?.GetDbTransaction().As<SqlTransaction>();

            if (parameters?.Any() ?? false)
                command.Parameters.AddRange(parameters);

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string commandText, SqlParameter[]? parameters)
             where T : class
        {
            if (SqlConnection.State == ConnectionState.Closed)
                SqlConnection.Open();

            using var command = SqlConnection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = DbContextTransaction?.GetDbTransaction().As<SqlTransaction>();

            if (parameters?.Any() ?? false)
                command.Parameters.AddRange(parameters);

            var datas = await command.ExecuteReaderAsync();
            if (!datas.HasRows)
            {
                return Enumerable.Empty<T>();
            }
            else
            {
                using DataTable table = new();
                table.Load(datas);
                var res = JsonConvert.DeserializeObject<IEnumerable<T>>(JsonConvert.SerializeObject(table));
                return null == res ? Enumerable.Empty<T>() : res;
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string commandText, SqlParameter[]? parameters)
            where T : class
        {
            if (SqlConnection.State == ConnectionState.Closed)
                SqlConnection.Open();

            using var command = SqlConnection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = DbContextTransaction?.GetDbTransaction().As<SqlTransaction>();

            if (parameters?.Any() ?? false)
                command.Parameters.AddRange(parameters);

            var datas = await command.ExecuteReaderAsync();
            if (!datas.HasRows)
            {
                return default(T);
            }
            else
            {
                using DataTable table = new();
                table.Load(datas);
                var res = JsonConvert.DeserializeObject<IEnumerable<T>>(JsonConvert.SerializeObject(table));
                return null == res 
                    ? default(T) 
                    : res.Count() > 1 
                        ? throw new ArsException("More than one sentence") 
                        : res.FirstOrDefault();
            }
        }

        public void Dispose()
        {
            DbContext?.Dispose();
            SqlConnection?.Close();
            SqlConnection?.Dispose();
            DbContextTransaction?.Dispose();
        }
    }
}
