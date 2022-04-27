using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyApiWithIdentityServer4
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>,ISingletonDependency
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            DbContextOptionsBuilder<MyDbContext> builder = new DbContextOptionsBuilder<MyDbContext>();
            string connectstring = configuration.GetSection("DefaultString").Get<string>();
            builder.UseMySql(connectstring,ServerVersion.AutoDetect(connectstring));

            return new MyDbContext(builder.Options);
        }
    }
}
