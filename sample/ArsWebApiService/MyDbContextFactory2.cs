using Ars.Common.Core.IDependency;
using ArsWebApiService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyApiWithIdentityServer4
{
    /// <summary>
    /// used by code first 
    /// </summary>
    public class MyDbContextFactory2 : IDesignTimeDbContextFactory<MyDbContext2>,ISingletonDependency
    {
        public MyDbContext2 CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<MyDbContext2> builder = new DbContextOptionsBuilder<MyDbContext2>();
            string connectstring = "Server=localhost;initial catalog=MYARS01;uid=root;pwd=123456;SslMode=none;TreatTinyAsBoolean=true;persistsecurityinfo=True;AllowPublicKeyRetrieval=true;";
            builder.UseMySql(connectstring,ServerVersion.AutoDetect(connectstring));

            return new MyDbContext2(builder.Options);
        }
    }
}
