using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Interfaces;
using Ars.Common.EFCore.DbContexts;
using Ars.Common.EFCore.Entities;
using Ars.Common.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DbContextTest
{


    public class MyTestDbContext : BaseArsDbContext<Guid, string>
    {
        public DbSet<Test> Tests => Set<Test>();

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
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string TenantId { get; set; } = Guid.NewGuid().ToString();
    }

    public class MyTest
    {
        private TestSession _testSession = new() { UserId = Guid.NewGuid() };
        private readonly EazyRepository<Test> _repository;
        private readonly MyTestDbContext _dbContext;

        public MyTest()
        {
            var options = new DbContextOptionsBuilder<MyTestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new MyTestDbContext(options, _testSession);
            _repository = new EazyRepository<Test>(_dbContext);
            CreateData();
        }

        private void CreateData()
        {
            for (int i = 1; i <= 20; i++)
            {
                _repository.Add(new Test
                {
                    Id = i,
                    Description = i.ToString(),
                });
            }
            _dbContext.SaveChanges();//实际使用时封装在工作单元内
        }

        [Fact]
        public void GetItems_ShouldReturnCount()
        {
            var rlt = _repository.GetAll().ToArray();
            Assert.Equal(20, rlt.Length);
        }

        [Fact]
        public void GetUserId_ShouldBeEqual()
        {
            var rlt = _repository.GetAll().ToArray();
            var arr = rlt.Select(t=>t.CreationUserId).ToArray();
            Assert.True(arr.Distinct().Count() <= 1);
        }
    }
}