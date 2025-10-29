using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Repository
{
    public interface IEazyRepository<TEntity> where TEntity : class
    {
        #region 异步方法 (Asynchronous Methods)

        /// <summary>
        /// 异步地根据主键获取实体。
        /// </summary>
        Task<TEntity?> GetByIdAsync(object id);

        /// <summary>
        /// 异步地根据条件查询实体集合。
        /// </summary>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步地添加单个实体。
        /// </summary>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// 异步地添加多个实体。
        /// </summary>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        #endregion 异步方法 (Asynchronous Methods)

        #region 同步方法 (Synchronous Methods)

        /// <summary>
        /// 同步地根据主键获取实体。
        /// </summary>
        TEntity? GetById(object id);

        /// <summary>
        /// 同步地根据条件查询实体集合。
        /// </summary>
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 同步地添加单个实体。
        /// </summary>
        void Add(TEntity entity);

        /// <summary>
        /// 同步地添加多个实体。
        /// </summary>
        void AddRange(IEnumerable<TEntity> entities);

        #endregion 同步方法 (Synchronous Methods)

        #region 通用方法 (Common Methods)

        /// <summary>
        /// 获取所有实体的 IQueryable 表达式，用于后续构建查询。
        /// </summary>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 标记一个实体为待移除状态。
        /// </summary>
        void Remove(TEntity entity);

        /// <summary>
        /// 标记多个实体为待移除状态。
        /// </summary>
        void RemoveRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// 标记一个实体为待更新状态。
        /// </summary>
        void Update(TEntity entity);

        #endregion 通用方法 (Common Methods)
    }
}
