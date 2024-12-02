using Ars.Common.Core.IDependency;
using ArsWebApiService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyApiWithIdentityServer4
{
    /// <summary>
    /// used by code first 
    /// </summary>
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>,ISingletonDependency
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<MyDbContext> builder = new DbContextOptionsBuilder<MyDbContext>();
            string connectstring = "Server=localhost;initial catalog=MYARS;uid=root;pwd=123456;SslMode=none;TreatTinyAsBoolean=true;persistsecurityinfo=True;AllowPublicKeyRetrieval=true;";
            builder.UseMySql(connectstring,ServerVersion.AutoDetect(connectstring));

            return new MyDbContext(builder.Options);
        }
    }
}
