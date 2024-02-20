using Ars.Common.EFCore;
using ArsWebApiService.Model.MyCatModel;
using Microsoft.EntityFrameworkCore;
using Task = ArsWebApiService.Model.MyCatModel.Task;

namespace ArsWebApiService
{
    public class MyCatDbContext : ArsDbContext
    {
        public MyCatDbContext(DbContextOptions<MyCatDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<User> t_user { get; set; }

        public DbSet<Product> t_product { get; set; }

        public DbSet<Task> t_task { get; set; }

        public DbSet<TaskDetail> t_task_detail { get; set; }

        public DbSet<Order> t_order { get; set; }

        public DbSet<OrderDetail> t_order_detail { get; set; }
    }
}
