using Ars.Common.AutoFac;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Extension;
using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore
{
    public abstract class ArsDbContext : DbContext,ITransientDependency
    {
        [Autowired]
        public IArsSession arsSession { get; set; }

        private static MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(ArsDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic)!;

        protected ArsDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
            where TEntity : class
        {
            if (null == mutableEntityType.BaseType && ShouldFilterEntity<TEntity>()) 
            {
                var filter = CreateQueryFilterExpression<TEntity>();
                if (null != filter) 
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filter);
                }
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>() 
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
                return true;

            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
                return true;

            return false;
        }

        protected virtual Expression<Func<TEntity, bool>>? CreateQueryFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> queryFilter = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> softfilter = e => ((ISoftDelete)e).IsDeleted == false;
                queryFilter = softfilter;
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity))) 
            {
                Expression<Func<TEntity, bool>> mayhavetenantFilter = e => ((IMayHaveTenant)e).TenantId == arsSession.TenantId;
                queryFilter = null == queryFilter ? mayhavetenantFilter : queryFilter.CombinExpression(mayhavetenantFilter);
            }

            return queryFilter;
        }
    }
}
