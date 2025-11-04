using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ars.Common.EFCore.Repository
{
    public class EazyRepository<TEntity> : IEazyRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EazyRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        #region 异步实现

        public virtual async Task<TEntity?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Context没释放可能直接从缓存中获取数据的问题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity?> GetByIdWithoutTrackerAsync(object id)
        {

            var keyName = _context.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties
                .Select(x => x.Name).Single();

            return await _dbSet.FirstOrDefaultAsync(e => EF.Property<object>(e, keyName).Equals(id));
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        #endregion 异步实现

        #region 同步实现

        public virtual TEntity? GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public virtual void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        #endregion 同步实现

        #region 通用实现

        public virtual IQueryable<TEntity> GetAll()
        {
            return _dbSet;
        }

        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        #endregion 通用实现
    }
}
