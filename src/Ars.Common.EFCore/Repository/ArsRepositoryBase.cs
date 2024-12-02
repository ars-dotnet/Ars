using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.EFCore.Entities;
using Ars.Common.EFCore.Extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Repository
{
    public abstract class ArsRepositoryBase<TDbContext, TEntity, TPrimaryKey> : IRepository<TDbContext, TEntity, TPrimaryKey>
          where TEntity : class, IEntity<TPrimaryKey>
          where TDbContext : DbContext
    {
        public abstract TDbContext GetDbContext();

        public abstract Task<TDbContext> GetDbContextAsync();

        public abstract IQueryable<TEntity> GetAll();

        public abstract Task<IQueryable<TEntity>> GetAllAsync();

        public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] selectors)
        {
            return GetAll();
        }

        public virtual Task<IQueryable<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity,object>>[] selectors) 
        {
            return GetAllAsync();
        }

        public virtual List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync() 
        {
            return Task.FromResult(GetAllList());
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(GetAllList(predicate));
        }

        public virtual TEntity? FirstOrDefault(Expression<Func<TEntity,bool>> predicate) 
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        public virtual Task<TEntity?> FirstOrDefaultAsync() 
        {
            return Task.FromResult(GetAll().FirstOrDefault());
        }

        public abstract TEntity Insert(TEntity entity);

        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        public abstract TEntity Update(TEntity entity);

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        public abstract void Delete(TEntity entity);

        public virtual Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.CompletedTask;
        }

        public virtual int Count()
        {
            return GetAll().Count();
        }

        public virtual Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count(predicate);
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        public virtual void Dispose() 
        {
            return;
        }

        public virtual Task<int> SaveChangesAsync()
        {
            return GetDbContext().SaveChangesAsync(); 
        }
    }
}
