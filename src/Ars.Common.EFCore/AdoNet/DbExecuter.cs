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
using NPOI.SS.Formula.Functions;
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
        }

        protected TDbContext DbContext { get; set; }

        protected DbConnection SqlConnection { get; set; }

        protected IDbContextTransaction? DbContextTransaction { get; set; }

        /// <summary>
        /// 使用EFCore事务
        /// </summary>
        /// <param name="dbContextTransaction"></param>
        /// <returns></returns>
        public void BeginWithEFCoreTransaction([NotNull] IActiveUnitOfWork activeUnitOfWork)
        {
            DbContext = activeUnitOfWork.GetDbContext<TDbContext>();
            SqlConnection = DbContext.Database.GetDbConnection();

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
            Init();

            DbContextTransaction = await DbContext.Database.BeginTransactionAsync(isolationLevel);
            return DbContextTransaction;
        }

        /// <summary>
        /// 查询列表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string commandText, DbParameter[]? parameters = null)
            where T : class, new()
        {
            Check();
            using var command = CreateCommond(commandText, parameters);

            using var datas = await command.ExecuteReaderAsync();
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

        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArsException"></exception>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(string commandText, DbParameter[]? parameters = null)
            where T : class, new()
        {
            Check();
            using var command = CreateCommond(commandText, parameters);

            using var datas = await command.ExecuteReaderAsync();
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

        /// <summary>
        /// 执行非查询sql语句
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQuery(string commandText, DbParameter[]? parameters = null)
        {
            Check();
            using var command = CreateCommond(commandText, parameters);

            return await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 返回结果中第一行的第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<T?> ExecuteScalarAsync<T>(string commandText, DbParameter[]? parameters = null)
             where T : struct
        {
            Check();
            using var command = CreateCommond(commandText, parameters);

            var data = await command.ExecuteScalarAsync();
            return data.As<T>();
        }

        public void Dispose()
        {
            SqlConnection?.Close();
            DbContextTransaction?.Dispose();
            DbContext?.Dispose();
        }

        protected void Init()
        {
            DbContext ??= _dbContextResolver.Resolve<TDbContext>();
            SqlConnection ??= DbContext.Database.GetDbConnection();
        }

        protected void Check()
        {
            Init();

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

    }
}
