using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Interfaces;
using Ars.Common.EFCore.DbContexts;
using Ars.Common.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace DbContextTest
{


    public class MyTestDbContext : BaseArsDbContext<Guid, string>
    {
        public MyTestDbContext(DbContextOptions dbContextOptions,
            IArsSession<Guid, string> session) : base(dbContextOptions, session)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Test : IModifyEntity<int, Guid, Guid>, IHaveTenant<string>, ISoftDelete
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? TenantId { get; set; }
        public Guid ModifyEntityId { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public Guid CreationUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class TestSession : IArsSession<Guid, string>
    {
        public Guid UserId { get; set; }
        public string TenantId { get; set; } = Guid.NewGuid().ToString();
    }

    public class MyTest
    {
        public MyTest() 
        {

        }
        [Fact]
        public void Test1()
        {

        }
    }
}