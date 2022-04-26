using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyApiWithIdentityServer4
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<MyDbContext> builder = new DbContextOptionsBuilder<MyDbContext>();
            string connectstring = "server=192.168.2.101;port=3306;user id=root;database=arsdatabase;password=FBx7ooOaWo;characterset=utf8;sslmode=none;";
            builder.UseMySql(connectstring,ServerVersion.AutoDetect(connectstring));

            return new MyDbContext(builder.Options);
        }
    }
}
