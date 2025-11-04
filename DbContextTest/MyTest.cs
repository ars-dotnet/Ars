using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Interfaces;
using Ars.Common.EFCore.DbContexts;
using Ars.Common.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace DbContextTest
{

    public class MyTestDbContext : BaseArsDbContext<string, string>
    {
        public MyTestDbContext(DbContextOptions dbContextOptions, IArsSession<string, string> session) : base(dbContextOptions, session)
        {
            //EnableGlobalFilters = false;
        }

        public DbSet<TestEntity> TestEntities { get; set; }
    }

    public class TestEntity : IModifyEntity<Guid, string, string>, ISoftDelete, IHaveTenant<string>
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string? Text { get; set; }
        public string? ModifyEntityId { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? CreationUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public string? TenantId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class TestSession : IArsSession<string, string>
    {
        public required string UserId { get; set; }
        public required string TenantId { get; set; }
    }

    public class MyTest
    {
        private readonly TestSession _tenantASession = new() { UserId = "user-A", TenantId = Guid.NewGuid().ToString() };
        private readonly TestSession _tenantBSession = new() { UserId = "user-B", TenantId = Guid.NewGuid().ToString() };

        private DbContextOptions<MyTestDbContext> _options = null!;

        public MyTest()
        {
            _options = new DbContextOptionsBuilder<MyTestDbContext>()
                .UseInMemoryDatabase("MySharedTestDatabase")
                .Options;

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var context = new MyTestDbContext(_options, _tenantASession);
            using var context1 = new MyTestDbContext(_options, _tenantBSession);

            for (int i = 1; i <= 10; i++)
            {
                context.TestEntities.Add(new TestEntity
                {
                    Number = i,
                    Text = $"Tenant A - Item {i}",
                });
            }

            for (int i = 1; i <= 10; i++)
            {
                context1.TestEntities.Add(new TestEntity
                {
                    Number = i,
                    Text = $"Tenant A - Item {i}",
                });
            }
            context.TestEntities.Add(new() { Number = 1024, Text = "Soft Delete Test" });
            context.SaveChanges();
            context1.SaveChanges();

        }

        [Fact]
        public void Query_WithGlobalFilterEnabled_ShouldOnlyReturnTenantSpecificAndNotDeletedData()
        {
            using var dbContextForTenantA = new MyTestDbContext(_options, _tenantASession);

            var results = dbContextForTenantA.TestEntities.ToArray();

            Assert.Equal(11, results.Length);
            Assert.True(results.All(e => e.TenantId == _tenantASession.TenantId), "All returned items must belong to Tenant A.");
            Assert.False(results.Any(e => e.Text == "Deleted Item"), "Soft-deleted items should not be returned.");
        }

        [Fact]
        public void Query_WithGlobalFilterDisabled_ShouldReturnAllData()
        {
            using var contextA = new MyTestDbContext(_options, _tenantASession);
            var entity = contextA.TestEntities.Where(t => t.Number == 1024).First();
            contextA.TestEntities.Remove(entity);
            contextA.SaveChanges();
            var rlt = contextA.TestEntities.ToArray();
            Assert.Equal(10, rlt.Length);

            var entity1 = contextA.TestEntities
                      .IgnoreQueryFilters() // <-- 临时忽略全局过滤器
                      .Where(t => t.Number == 1024)
                      .First();
            Assert.True(entity1.IsDeleted);
        }

        [Fact]
        public void AddEntity_ShouldAutomaticallySetTenantIdAndAuditFields()
        {
            using var context = new MyTestDbContext(_options, _tenantASession);
            var newEntity = new TestEntity { Number = 101, Text = "New Item" };

            context.TestEntities.Add(newEntity);
            context.SaveChanges();

            Assert.NotNull(newEntity.TenantId);
            Assert.Equal(_tenantASession.TenantId, newEntity.TenantId);

            Assert.NotNull(newEntity.CreationUserId);
            Assert.Equal(_tenantASession.UserId, newEntity.CreationUserId);

            Assert.NotEqual(default(DateTime), newEntity.CreationTime);
            Assert.NotEqual(Guid.Empty, newEntity.Id);
        }

        [Fact]
        public void Modify_ShouldAutomaticallySetAuditFields()
        {
            using var context = new MyTestDbContext(_options, _tenantASession);
            var entity = context.TestEntities.Where(t => t.Number == 1).First();
            entity.Text = "Modified Item";
            context.SaveChanges();
            var list = context.TestEntities.Where(t => t.ModifyEntityId != null).ToArray();
            Assert.True(list.All(t => t.ModifyEntityId == _tenantASession.UserId), "LastUpdatedBy should match the session's UserId.");
        }
    }
}