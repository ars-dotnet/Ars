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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.EFCore.AdoNet
{
    internal class DbExecuter<TDbContext> : IDbExecuter<TDbContext>
        where TDbContext : DbContext
    {
        private readonly IDbContextResolver _dbContextResolver;
        public DbExecuter(IDbContextResolver dbContextResolver)
        {
            _dbContextResolver = dbContextResolver;
            //DbContext = dbContextResolver.Resolve<TDbContext>();
        }

        protected TDbContext DbContext { get; set; }

        protected DbConnection SqlConnection => DbContext.Database.GetDbConnection();

        protected IDbContextTransaction? DbContextTransaction { get; set; }

        /// <summary>
        /// 使用EFCore事务
        /// </summary>
        /// <param name="dbContextTransaction"></param>
        /// <returns></returns>
        public void BeginWithEFCoreTransaction([NotNull] IActiveUnitOfWork activeUnitOfWork)
        {
            DbContext = activeUnitOfWork.GetDbContext<TDbContext>();
            DbContextTransaction = activeUnitOfWork.GetContextTransaction<TDbContext>();

            return;
        }

        /// <summary>
        /// 新建一个ado.net事务
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.RepeatableRead)
        {
            DbContext ??= _dbContextResolver.Resolve<TDbContext>();
            DbContextTransaction = await DbContext.Database.BeginTransactionAsync(isolationLevel);
            return DbContextTransaction;
        }

        protected void Init() 
        {
            DbContext ??= _dbContextResolver.Resolve<TDbContext>();
            if (SqlConnection.State == ConnectionState.Closed)
                SqlConnection.Open();
        }

        protected DbCommand CreateCommond(string commandText, DbParameter[]? parameters = null) 
        {
            var command = SqlConnection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = DbContextTransaction?.GetDbTransaction();

            if (parameters?.Any() ?? false)
                command.Parameters.AddRange(parameters);

            return command;
        }

        public async Task<int> ExecuteNonQuery(string commandText, DbParameter[]? parameters = null)
        {
            Init();
            using var command = CreateCommond(commandText, parameters);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string commandText, DbParameter[]? parameters = null)
             where T : class, new()
        {
            Init();
            using var command = CreateCommond(commandText, parameters);

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
                return !res.HasValue() ? Enumerable.Empty<T>() : res!;
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string commandText, DbParameter[]? parameters = null)
            where T : class, new()
        {
            Init();
            using var command = CreateCommond(commandText, parameters);

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

        public async Task<T?> ExecuteScalarAsync<T>(string commandText, DbParameter[]? parameters = null)
             where T : struct
        {
            Init();
            using var command = CreateCommond(commandText, parameters);

            var data = await command.ExecuteScalarAsync();
            return data.As<T>();
        }

        public void Dispose()
        {
            //if (null != DbContext?.Database)
            //{
            //    SqlConnection?.Close();
            //    SqlConnection?.Dispose();
            //}

            DbContextTransaction?.Dispose();
            DbContext?.Dispose();
        }
    }
}
