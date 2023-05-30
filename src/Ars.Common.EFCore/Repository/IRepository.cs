using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Repository
{
    public interface IRepository : IDisposable
    {

    }

    public interface IRepository<TEntity, TPrimaryKey> : IRepository 
        where TEntity : class, IEntity<TPrimaryKey>
    {
        IQueryable<TEntity> GetAll();

        Task<IQueryable<TEntity>> GetAllAsync();

        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] selectors);

        Task<IQueryable<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] selectors);

        List<TEntity> GetAllList();

        Task<List<TEntity>> GetAllListAsync();

        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Insert(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);

        int Count();

        Task<int> CountAsync();

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }

    public interface IRepository<TEntity> : IRepository<TEntity, int> 
        where TEntity : class, IEntity<int> 
    {

    }
}
