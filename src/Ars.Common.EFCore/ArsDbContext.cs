using Ars.Common.AutoFac;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Entities;
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
using Ars.Common.Core.Uow.Options;
using Ars.Common.Core.Configs;
using Ars.Common.Tool.Extension;
using Ars.Common.Core.AspNetCore.Extensions;
using Newtonsoft.Json.Linq;
using Ars.Common.Core.Diagnostic;
using NPOI.SS.Formula.Functions;

namespace Ars.Common.EFCore
{
    public abstract class ArsDbContext : DbContext,ITransientDependency
    {
        private readonly IArsSession? _arsSession;

        private readonly IArsDbContextConfiguration? _options;

        private static MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(ArsDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic)!;

        protected ArsDbContext(
            DbContextOptions dbContextOptions,
            IArsSession? arsSession,
            IArsDbContextConfiguration? options) : base(dbContextOptions)
        {
            _arsSession = arsSession;
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (_options?.UseLazyLoadingProxies ?? false)
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
            if (typeof(IEntity<>).IsAssignableFrom(typeof(TEntity)))
                return true;

            if (typeof(ICreateEntity).IsAssignableFrom(typeof(TEntity)))
                return true;

            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
                return true;

            return false;
        }

        protected virtual bool ShouldSetValueFilterEntity(Type type)
        {
            if (typeof(IEntity<>).IsAssignableGenericFrom(type))
                return true;

            if (typeof(ICreateEntity).IsAssignableFrom(type))
                return true;

            if (typeof(IMayHaveTenant).IsAssignableFrom(type))
                return true;

            return false;
        }

        protected virtual Expression<Func<TEntity, bool>>? CreateQueryFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>>? queryFilter = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> softfilter = e => ((ISoftDelete)e).IsDeleted == false;
                queryFilter = softfilter;
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>>? mayhavetenantFilter = null;
                if (null == _arsSession)
                    mayhavetenantFilter = e => true;
                else
                {
                    int? sessionid = _arsSession.TenantId;
                    mayhavetenantFilter = e => sessionid.HasValue ? ((IMayHaveTenant)e).TenantId == sessionid : true;
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

                    AddChangerTables(entry);
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

                    AddChangerTables(entry);
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
                       r.TargetEntry != null &&
                       r.TargetEntry.Metadata.IsOwned() &&
                       CheckOwnedEntityChange(r.TargetEntry));
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
            if (ShouldSetValueFilterEntity(entityEntry.Entity.GetType()))
            {
                CheckAndSetId(entityEntry);
                CheckAndSetMayHaveTenantIdProperty(entityEntry);
                CheckAndSetCreationProperty(entityEntry);
            }
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
            if (null != entity && !entity.TenantId.HasValue)
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
                    entity.CreationTime = DateTime.Now;
            }
        }

        #endregion add-set

        protected virtual void ConceptsForModifyEntity(EntityEntry entityEntry)
        {
            SetModificationAuditProperties(entityEntry);
        }

        protected virtual void SetModificationAuditProperties(EntityEntry entityEntry)
        {
            if (!entityEntry.Entity.Is<IModifyEntity>())
                return;

            var entity = entityEntry.Entity.As<IModifyEntity>();
            if (null != entity)
            {
                if (!entity.UpdateUserId.HasValue)
                    entity.UpdateUserId = _arsSession?.UserId;
                if (!entity.UpdateTime.HasValue)
                    entity.UpdateTime = DateTime.Now;
            }
        }

        protected virtual void ConceptsForDeleteEntity(EntityEntry entityEntry)
        {
            CancelDeletionForSoftDelete(entityEntry);
        }

        protected virtual void CancelDeletionForSoftDelete(EntityEntry entityEntry)
        {
            if (entityEntry.Entity.Is<ISoftDelete>())
            {
                entityEntry.Reload();
                entityEntry.State = EntityState.Modified;
                entityEntry.Entity.As<ISoftDelete>()!.IsDeleted = true;
            }

            if (entityEntry.Entity.Is<IDeleteEntity>())
            {
                var entity = entityEntry.Entity.As<IDeleteEntity>()!;
                if (!entity.DeleteUserId.HasValue)
                    entity.DeleteUserId = _arsSession?.UserId;
                if (!entity.DeleteTime.HasValue)
                    entity.DeleteTime = DateTime.Now;
            }
        }

        private IList<ChangerTable> _changerTables;

        protected virtual void AddChangerTables(EntityEntry entityEntry) 
        {
            switch(entityEntry.State) 
            {
                case EntityState.Added:  
                case EntityState.Modified:
                case EntityState.Deleted:
                    CreateChangerTable(entityEntry);
                    break;
            }
        }

        protected virtual void CreateChangerTable(EntityEntry entityEntry) 
        {
            ChangerTable changerTable = new ChangerTable
            {
                TableName = entityEntry.Metadata.GetDefaultTableName() 
                         ?? entityEntry.Entity.GetType().Name,
                EntityEntry = entityEntry,
                EntityState = GetDiagnosticEntityState(entityEntry.State),
                OriginalValues = 
                    entityEntry.State == EntityState.Added 
                    ? null
                    : GetOriginalEntryValue(entityEntry),
                CurrentValues = 
                    entityEntry.State == EntityState.Deleted 
                    ? null
                    : GetCurrentEntryValue(entityEntry),
            };
            _changerTables ??= new List<ChangerTable>();
            _changerTables.Add(changerTable);
        }

        private JObject GetOriginalEntryValue(EntityEntry entityEntry) 
        {
            JObject value = new JObject();
            foreach (var property in entityEntry.Properties) 
            {
                value[property.Metadata.Name] = null == property.OriginalValue 
                    ? JValue.CreateNull()
                    : JToken.FromObject(property.OriginalValue);
            }
            return value;
        }

        private JObject GetCurrentEntryValue(EntityEntry entityEntry)
        {
            JObject value = new JObject();
            foreach (var property in entityEntry.Properties)
            {
                value[property.Metadata.Name] = null == property.CurrentValue
                    ? JValue.CreateNull()
                    : JToken.FromObject(property.CurrentValue);
            }
            return value;
        }

        public virtual IEnumerable<ChangerTable> GetChangerTables() 
        {
            if (_changerTables.HasNotValue())
                return _changerTables;

            //自增主键重新赋值
            var tables = from table in _changerTables
                              let keys = GetPrimaryKeys(table.EntityEntry)
                         from key in keys
                         where 
                              table.EntityState == DiagnosticEntityState.Added &&
                              null != table.CurrentValues &&
                              keys.HasValue() &&
                              (!key.Item2.ToString()?.Equals(table.CurrentValues![key.Item1]?.ToString()) ?? false)
                         select new { table, primaryKeyValue = key };
            foreach (var item in tables)
            {
                item.table.CurrentValues![item.primaryKeyValue.Item1] = JToken.FromObject(item.primaryKeyValue.Item2);
            }
            
            return _changerTables;
        }

        public DiagnosticEntityState GetDiagnosticEntityState(EntityState entityState)
        {
            switch (entityState) 
            {
                case EntityState.Added:
                    return DiagnosticEntityState.Added;
                case EntityState.Modified:
                    return DiagnosticEntityState.Modified;
                case EntityState.Deleted:
                    return DiagnosticEntityState.Deleted;
                default:
                    return DiagnosticEntityState.Deleted;
            }
        }

        private IEnumerable<(string, object?)> GetPrimaryKeys(EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            if (key?.Properties.Any() ?? false)
            {
                foreach (var property in key.Properties)
                {
                    yield return (property.Name, entry.Property(property.Name).CurrentValue);
                }
            }
        }

        public override void Dispose()
        {
            _changerTables?.Clear();
            base.Dispose();
        }
    }
}
