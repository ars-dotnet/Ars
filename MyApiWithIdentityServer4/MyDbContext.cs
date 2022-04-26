using Ars.Common.EFCore;
using Microsoft.EntityFrameworkCore;
using MyApiWithIdentityServer4.Model;

namespace MyApiWithIdentityServer4
{
    public class MyDbContext : ArsDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
    }
}
