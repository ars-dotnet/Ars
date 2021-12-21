using ExpressionDemo.MyDbFunctionsExtensions;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionDemo
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> dbContext) : base(dbContext)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Users>().ToTable("Users");
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseDmAlgorithmsEncryptionFunctions();
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySql("Server=192.168.1.229;initial catalog=oline_clientconfig_upgrade;uid=update;pwd=update!@#$%;SslMode=none;TreatTinyAsBoolean=true;", ServerVersion.Parse("8.0.21"));
        }

        public DbSet<Users> Users { set; get; }
    }
}
