using Ars.Common.AutoFac;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Extension;
using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Entities;
using Ars.Common.EFCore.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ars.Commom.Tool.Extension;

namespace Ars.Common.EFCore
{
    public abstract class ArsDbContext : DbContext
    {
        private readonly IArsSession? _arsSession;

        private readonly DbContextOption? _options;

        private static MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(ArsDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic)!;

        protected ArsDbContext(DbContextOptions dbContextOptions, IArsSession? arsSession, DbContextOption? options) : base(dbContextOptions)
        {
            _arsSession = arsSession;
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (_options?.UseLazyLoadingProxies ?? true)
                optionsBuilder.UseLazyLoadingProxies();
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

        protected virtual bool ShouldSetValueFilterEntity<TEntity>()
        {
            if (typeof(IEntity).IsAssignableFrom(typeof(TEntity)))
                return true;

            if (typeof(ICreateEntity).IsAssignableFrom(typeof(TEntity)))
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
                Expression<Func<TEntity, bool>> mayhavetenantFilter = null;
                if (null == _arsSession)
                    mayhavetenantFilter = e => true;
                else 
                {
                    int? sessionid = _arsSession.TenantId;
                    mayhavetenantFilter = e => ((IMayHaveTenant)e).TenantId == sessionid;
                }

                queryFilter = null == queryFilter ? mayhavetenantFilter : queryFilter.CombinExpression(mayhavetenantFilter);
            }

            return queryFilter;
        }

        public override int SaveChanges()
        {
            try
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.State != EntityState.Modified && CheckOwnedEntityChange(entry))
                    {
                        entry.State = EntityState.Modified;
                    }

                    ConceptEntry(entry);
                }

                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.State != EntityState.Modified && CheckOwnedEntityChange(entry))
                    { 
                        entry.State = EntityState.Modified;
                    }

                    ConceptEntry(entry);
                }

                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) 
            {
                throw;
            }
        }

        protected virtual bool CheckOwnedEntityChange(EntityEntry entry) 
        {
            return entry.State == EntityState.Modified ||
                   entry.References.Any(r =>
                       r.TargetEntry != null && r.TargetEntry.Metadata.IsOwned() && CheckOwnedEntityChange(r.TargetEntry));
        }

        protected virtual void ConceptEntry(EntityEntry entityEntry) 
        {
            switch (entityEntry.State) 
            {
                case EntityState.Added:
                    ConceptsForAddEntity(entityEntry);
                    break;
                case EntityState.Modified:
                    ConceptsForModifyEntity(entityEntry);
                    break;
                case EntityState.Deleted:
                    ConceptsForDeleteEntity(entityEntry);
                    break;
            }
        }

        #region add-set
        protected virtual void ConceptsForAddEntity(EntityEntry entityEntry) 
        {
            CheckAndSetId(entityEntry);
            CheckAndSetMayHaveTenantIdProperty(entityEntry);
            CheckAndSetCreationProperty(entityEntry);
        }

        protected virtual void CheckAndSetId(EntityEntry entry) 
        {
            var entity = entry.Entity.As<IEntity<Guid>>();
            if (null != entity && entity.Id == Guid.Empty) 
            {
                var idPropertyEntry = entry.Property("Id");

                if (idPropertyEntry != null && idPropertyEntry.Metadata.ValueGenerated == ValueGenerated.Never)
                {
                    entity.Id = Guid.NewGuid();
                }
            }
        }

        protected virtual void CheckAndSetMayHaveTenantIdProperty(EntityEntry entry) 
        {
            var entity = entry.Entity.As<IMayHaveTenant>();
            if(null != entity && !entity.TenantId.HasValue)
            {
                entity.TenantId = _arsSession?.TenantId;
            }
        }

        protected virtual void CheckAndSetCreationProperty(EntityEntry entry) 
        {
            var entity = entry.Entity.As<ICreateEntity>();
            if (null != entity) 
            {
                if (!entity.CreationUserId.HasValue)
                    entity.CreationUserId = _arsSession?.UserId;
                if (!entity.CreationTime.HasValue)
                    entity.CreationTime = DateTime.UtcNow;
            }
        }

        #endregion add-set

        protected virtual void ConceptsForModifyEntity(EntityEntry entityEntry)
        {
            SetModificationAuditProperties(entityEntry);
        }

        protected virtual void SetModificationAuditProperties(EntityEntry entityEntry) 
        {
            var entity = entityEntry.Entity.As<IModifyEntity>();
            if(null != entity)
            {
                if(!entity.UpdateUserId.HasValue)
                    entity.UpdateUserId = _arsSession?.UserId;
                if(!entity.UpdateTime.HasValue)
                    entity.UpdateTime = DateTime.UtcNow;
            }
        }

        protected virtual void ConceptsForDeleteEntity(EntityEntry entityEntry)
        {
            CancelDeletionForSoftDelete(entityEntry);
        }

        protected virtual void CancelDeletionForSoftDelete(EntityEntry entityEntry) 
        {
            if (!(entityEntry.Entity is ISoftDelete))
            {
                return;
            }

            entityEntry.Reload();
            entityEntry.State = EntityState.Modified;
            entityEntry.Entity.As<ISoftDelete>().IsDeleted = true;

            var entity = entityEntry.Entity.As<IDeleteEntity>();
            if (null != entity)
            {
                if (!entity.DeleteUserId.HasValue)
                    entity.DeleteUserId = _arsSession?.UserId;
                if (!entity.DeleteTime.HasValue)
                    entity.DeleteTime = DateTime.UtcNow;
            }
        }
    }
}
