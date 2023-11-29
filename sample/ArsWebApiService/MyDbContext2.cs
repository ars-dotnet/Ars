using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Uow.Options;
using Ars.Common.EFCore;
using ArsWebApiService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyApiWithIdentityServer4.Model;

namespace ArsWebApiService
{
    public class MyDbContext2 : ArsDbContext
    {
        public MyDbContext2(DbContextOptions<MyDbContext2> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<StudentNew> StudentNew { get; set; }
    }
}
