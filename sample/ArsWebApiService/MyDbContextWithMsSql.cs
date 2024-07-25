using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Configs;
using Ars.Common.EFCore;
using ArsWebApiService.Model;
using Microsoft.EntityFrameworkCore;
using MyApiWithIdentityServer4.Model;

namespace ArsWebApiService
{
    public class MyDbContextWithMsSql : ArsDbContext
    {
        public MyDbContextWithMsSql(DbContextOptions<MyDbContextWithMsSql> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<StudentMsSql>().OwnsMany(student => student.JsonProperty,
            //    ownedNavigationBuilder => ownedNavigationBuilder.ToJson());
        }

        public DbSet<StudentMsSql> Students { get; set; }
    }
}
