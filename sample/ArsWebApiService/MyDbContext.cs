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
    public class MyDbContext : ArsDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().HasMany(r => r.Enrollments).WithOne(r => r.Student);
            modelBuilder.Entity<Course>().HasMany(r => r.Enrollments).WithOne(r => r.Course);
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentNew> StudentNew { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<ClassRoom> ClassRoom { get; set; }

        public DbSet<AppVersion> AppVersion { get; set; }
    }
}
