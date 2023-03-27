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
    public class EfCoreRepositoryBase<TDbContext, TEntity, TPrimaryKey> : ArsRepositoryBase<TDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {

        [Autowired]
        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        protected virtual TDbContext GetDbContext() => CurrentUnitOfWorkProvider.Current!.GetDbContext<TDbContext>();

        protected virtual Task<TDbContext> GetDbContextAsync() => CurrentUnitOfWorkProvider.Current!.GetDbContextAsync<TDbContext>();

        protected virtual DbSet<TEntity> GetTable() => GetDbContext().Set<TEntity>();

        protected virtual async Task<DbSet<TEntity>> GetTableAsync() => (await GetDbContextAsync()).Set<TEntity>();

        public override IQueryable<TEntity> GetAll()
        {
            return GetDbContext().Set<TEntity>();
        }

        public override async Task<IQueryable<TEntity>> GetAllAsync()
        {
            return (await GetDbContextAsync()).Set<TEntity>();
        }

        public override IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] selectors)
        {
            var query = GetAll();
            if (null == selectors)
                return query;

            foreach (var selector in selectors)
            {
                query = query.Include(selector);
            }

            return query;
        }

        public override async Task<IQueryable<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] selectors)
        {
            var query = await GetAllAsync();
            if (null == selectors)
                return query;

            foreach (var selector in selectors)
            {
                query = query.Include(selector);
            }

            return query;
        }

        public override async Task<List<TEntity>> GetAllListAsync()
        {
            return await (await GetAllAsync()).ToListAsync();
        }

        public override async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await (await GetAllAsync()).Where(predicate).ToListAsync();
        }

        public override async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await (await GetAllAsync()).FirstOrDefaultAsync(predicate);
        }

        public override TEntity Insert(TEntity entity)
        {
            return GetTable().Add(entity).Entity;
        }

        public override async Task<TEntity> InsertAsync(TEntity entity)
        {
            return (await (await GetTableAsync()).AddAsync(entity)).Entity;
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = GetDbContext().ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            GetTable().Attach(entity);
        }

        public override TEntity Update(TEntity entity)
        {
            return GetTable().Update(entity).Entity;
        }

        public override Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        public override void Delete(TEntity entity)
        {
            GetTable().Remove(entity);
        }

        public override async Task<int> CountAsync()
        {
            return await (await GetAllAsync()).CountAsync();
        }

        public override async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await (await GetAllAsync()).CountAsync(predicate);
        }
    }

    public class EfCoreRepositoryBase<TDbContext, TEntity> : EfCoreRepositoryBase<TDbContext, TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {

    }
}
