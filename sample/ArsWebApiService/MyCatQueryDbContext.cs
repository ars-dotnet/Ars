using Ars.Common.EFCore;
using ArsWebApiService.Model.MyCatModel;
using Microsoft.EntityFrameworkCore;

namespace ArsWebApiService
{
    public class MyCatQueryDbContext : ArsDbContext
    {
        public MyCatQueryDbContext(DbContextOptions<MyCatQueryDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<OrderQuery> t_order { get; set; }

        public DbSet<OrderDetailQuery> t_order_detail { get; set; }
    }
}
