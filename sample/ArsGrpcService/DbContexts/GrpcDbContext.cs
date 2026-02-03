using Ars.Common.EFCore.DbContexts;
using ArsWebApiService.Model;
using Microsoft.EntityFrameworkCore;

namespace ArsGrpcService.DbContexts
{
    public class GrpcDbContext : ArsDbContext
    {
        public GrpcDbContext(DbContextOptions<GrpcDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }


        public DbSet<StudentNew> StudentNew { get; set; }
    }
}
