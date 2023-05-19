using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.AdoNet
{
    public interface IDbExecuter<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        /// <summary>
        /// 使用EFCore事务
        /// </summary>
        /// <param name="activeUnitOfWork"></param>
        /// <returns></returns>
        void BeginWithEFCoreTransaction([NotNull] IActiveUnitOfWork activeUnitOfWork);

        /// <summary>
        /// 新建一个ado.net事务
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.RepeatableRead);

        /// <summary>
        /// 执行非查询sql语句
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<int> ExecuteNonQuery(string commandText, DbParameter[]? parameters = null);

        /// <summary>
        /// 查询列表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string commandText, DbParameter[]? parameters = null)
             where T : class,new();

        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<T?> QueryFirstOrDefaultAsync<T>(string commandText, DbParameter[]? parameters = null)
             where T : class,new();

        /// <summary>
        /// 返回结果中第一行的第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<T?> ExecuteScalarAsync<T>(string commandText, DbParameter[]? parameters = null)
             where T : struct;
    }
}
