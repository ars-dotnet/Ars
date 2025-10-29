using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Diagnostic;
using Ars.Common.Core.Extensions;
using Ars.Common.Core.Interfaces;
using Ars.Common.EFCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ars.Common.EFCore.DbContexts
{
    /// <summary>
    /// 这里不管是否使用懒加载,需要可以继承复写，其次避免库污染
    /// </summary>
    /// <typeparam name="TUserId"></typeparam>
    /// <typeparam name="KTenantId"></typeparam>
    public abstract class BaseArsDbContext<TUserId, KTenantId> : DbContext where TUserId : notnull
    {
        public IArsSession<TUserId, KTenantId> ArsSession { get; }

        private IList<EFCoreChangerTable>? _infos;

        //private static readonly ConcurrentDictionary<Type, Action<object, TUserId>?> UserIdSetters = new();实际测试没有性能影响，这里不做缓存
        //private static readonly ConcurrentDictionary<Type, Action<object, DateTime>?> DateTimeSetters = new();
        private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo =
            typeof(BaseArsDbContext<TUserId, KTenantId>).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic)!;

        protected BaseArsDbContext(DbContextOptions dbContextOptions, IArsSession<TUserId, KTenantId> session)
            : base(dbContextOptions)
        {
            ArsSession = session;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, [modelBuilder, entityType]);
            }
        }

        #region filter

        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class
        {
            if (mutableEntityType.BaseType == null && ShouldFilterEntity<TEntity>())
            {
                var filter = CreateQueryFilterExpression(typeof(TEntity));
                if (filter != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filter);
                }
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>()
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity))) return true;
            if (typeof(IHaveTenant<KTenantId>).IsAssignableFrom(typeof(TEntity))) return true;
            return false;
        }

        protected virtual LambdaExpression? CreateQueryFilterExpression(Type entityType)
        {
            Expression? finalExpression = null;
            var parameter = Expression.Parameter(entityType, "e");

            if (typeof(ISoftDelete).IsAssignableFrom(entityType))
            {
                var isDeletedProperty = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                finalExpression = Expression.Equal(isDeletedProperty, Expression.Constant(false));
            }

            if (typeof(IHaveTenant<>).MakeGenericType(typeof(KTenantId)).IsAssignableFrom(entityType))
            {
                var sessionProperty = Expression.Property(Expression.Constant(this), nameof(this.ArsSession));
                var sessionTenantId = Expression.Property(sessionProperty, nameof(this.ArsSession.TenantId));
                var entityTenantIdProperty = Expression.Property(parameter, nameof(IHaveTenant<KTenantId>.TenantId));
                var tenantFilter = Expression.Equal(entityTenantIdProperty, Expression.Convert(sessionTenantId, entityTenantIdProperty.Type));

                finalExpression = finalExpression == null ? tenantFilter : Expression.AndAlso(finalExpression, tenantFilter);
            }

            return finalExpression != null ? Expression.Lambda(finalExpression, parameter) : null;
        }

        #endregion filter

        #region Create ChangeInfo

        protected virtual void AddChangeInfos(EntityEntry entityEntry)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                case EntityState.Deleted:
                    CreateChangeInfo(entityEntry);
                    break;
            }
        }

        protected virtual void CreateChangeInfo(EntityEntry entityEntry)
        {
            EFCoreChangerTable ChangeInfo = new()
            {
                TableName = entityEntry.Metadata.GetDefaultTableName()
                         ?? entityEntry.Entity.GetType().Name,
                EntityState = GetDiagnosticEntityState(entityEntry.State),
                OriginalValues =
                    entityEntry.State == EntityState.Added
                    ? null
                    : GetOriginalEntryValue(entityEntry),
                CurrentValues =
                    entityEntry.State == EntityState.Deleted
                    ? null
                    : GetCurrentEntryValue(entityEntry),

                EntityEntry = entityEntry,
            };
            _infos ??= [];
            _infos.Add(ChangeInfo);
        }

        private static JObject GetOriginalEntryValue(EntityEntry entityEntry)
        {
            JObject value = [];
            foreach (var property in entityEntry.Properties)
            {
                value[property.Metadata.Name] = null == property.OriginalValue
                    ? JValue.CreateNull()
                    : JToken.FromObject(property.OriginalValue);
            }
            return value;
        }

        private static JObject GetCurrentEntryValue(EntityEntry entityEntry)
        {
            JObject value = [];
            foreach (var property in entityEntry.Properties)
            {
                value[property.Metadata.Name] = null == property.CurrentValue
                    ? JValue.CreateNull()
                    : JToken.FromObject(property.CurrentValue);
            }
            return value;
        }

        #endregion Create ChangeInfo

        protected virtual bool CheckOwnedEntityChange(EntityEntry entry)
        {
            return entry.State == EntityState.Modified ||
                   entry.References.Any(r =>
                       r.TargetEntry != null &&
                       r.TargetEntry.Metadata.IsOwned() &&
                       CheckOwnedEntityChange(r.TargetEntry));
        }

        #region SaveChanges Overrides

        public override int SaveChanges()
        {
            try
            {
                ProcessTrackedEntries();
                return base.SaveChanges();
            }
            finally
            {
                _infos?.Clear();
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ProcessTrackedEntries();
                return await base.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                _infos?.Clear();
            }
        }

        #endregion SaveChanges Overrides

        #region Entry Processing Logic

        private void ProcessTrackedEntries()
        {
            var now = DateTime.UtcNow;

            SetTenantIdOnNewEntities();

            foreach (var entry in ChangeTracker.Entries().Where(e => e.State != EntityState.Detached && e.State != EntityState.Unchanged))
            {
                if (entry.State != EntityState.Modified && CheckOwnedEntityChange(entry))
                {
                    entry.State = EntityState.Modified;
                }

                ConceptEntry(entry, now);
                AddChangeInfos(entry);
            }
        }

        protected virtual void ConceptEntry(EntityEntry entityEntry, DateTime now)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    ConceptsForAddEntity(entityEntry, now);
                    break;

                case EntityState.Modified:
                    ConceptsForModifyEntity(entityEntry, now);
                    break;

                case EntityState.Deleted:
                    ConceptsForDeleteEntity(entityEntry, now);
                    break;
            }
        }

        #endregion Entry Processing Logic

        #region Default Property Setters

        private void SetUserIdProperty(object entity, string propertyName, bool onlySetIfDefault = false)
        {
            var propertyInfo = entity.GetType().GetProperty(propertyName);
            if (propertyInfo == null || ArsSession == null) return;

            if (onlySetIfDefault)
            {
                var currentValue = propertyInfo.GetValue(entity);
                if (currentValue != null && !currentValue.Equals(default)) return;
            }

            if (propertyInfo.PropertyType.IsAssignableFrom(typeof(TUserId)))
            {
                propertyInfo.SetValue(entity, ArsSession.UserId);
            }
            else
            {
                try
                {
                    var targetType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    var convertedValue = Convert.ChangeType(ArsSession.UserId, targetType);
                    propertyInfo.SetValue(entity, convertedValue);
                }
                //catch (Exception)
                //{
                //    //看实际情况添加处理
                //}
                finally
                {
                }
            }
        }

        private void SetDateTimeProperty(object entity, string propertyName, DateTime value, bool onlySetIfDefault = false)
        {
            var propertyInfo = entity.GetType().GetProperty(propertyName);
            if (propertyInfo == null || !(propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))) return;

            if (onlySetIfDefault)
            {
                var currentValue = propertyInfo.GetValue(entity);
                if (currentValue != null && ((DateTime)currentValue) != default) return;
            }
            propertyInfo.SetValue(entity, value);
        }

        protected virtual void ConceptsForDeleteEntity(EntityEntry entityEntry, DateTime now)
        {
            if (entityEntry.Entity is ISoftDelete softDeleteEntity)
            {
                entityEntry.State = EntityState.Modified;
                softDeleteEntity.IsDeleted = true;
            }

            if (entityEntry.Entity.GetType().IsAssignableToGenericType(typeof(IDeleteEntity<>)))
            {
                SetUserIdProperty(entityEntry.Entity, "DeleteUserId");
                SetDateTimeProperty(entityEntry.Entity, "DeleteTime", now);
            }
        }

        protected virtual void ConceptsForModifyEntity(EntityEntry entityEntry, DateTime now)
        {
            var entityType = entityEntry.Entity.GetType();
            if (entityType.IsAssignableToGenericType(typeof(IModifyEntity<,>)) ||
                entityType.IsAssignableToGenericType(typeof(IModifyEntity<,,>)))
            {
                SetUserIdProperty(entityEntry.Entity, "ModifyEntityId");
                SetDateTimeProperty(entityEntry.Entity, "ModifiedTime", now);
            }
        }

        protected virtual void ConceptsForAddEntity(EntityEntry entityEntry, DateTime now)
        {
            CheckAndSetId(entityEntry);
            var entityType = entityEntry.Entity.GetType();

            if (entityType.IsAssignableToGenericType(typeof(ICreateEntity<,>)))
            {
                SetUserIdProperty(entityEntry.Entity, "CreationUserId", onlySetIfDefault: true);
                SetDateTimeProperty(entityEntry.Entity, "CreationTime", now, onlySetIfDefault: true);
            }
        }

        private void SetTenantIdOnNewEntities()
        {
            foreach (var entry in ChangeTracker.Entries<IHaveTenant<KTenantId>>().Where(e => e.State == EntityState.Added))
            {
                entry.Entity.TenantId = ArsSession.TenantId;
            }
        }

        protected virtual void CheckAndSetId(EntityEntry entry)
        {
            if (entry.Entity is Core.Interfaces.IEntity<Guid> entity && entity.Id == Guid.Empty)
            {
                var idPropertyEntry = entry.Property("Id");
                if (idPropertyEntry != null && idPropertyEntry.Metadata.ValueGenerated == ValueGenerated.Never)
                {
                    entity.Id = Guid.NewGuid();
                }
            }
        }

        #endregion Default Property Setters

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
                    //default:
                    //throw new InvalidOperationException($"Unsupported EntityState: {entityState}");
            }
        }
    }
}
