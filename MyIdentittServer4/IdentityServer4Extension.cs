using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using MyIdentittServer4.Configs;

namespace MyIdentittServer4
{
    public static class IdentityServer4Extension
    {
        public static void AddMyidentityserver(this IServiceCollection services) 
        {
            var builder = services.AddIdentityServer(options => 
            {
                options.UserInteraction = new UserInteractionOptions
                {
                    LoginUrl = "/user/login",
                    LogoutUrl = "/user/logout",
                };
            });

            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddInMemoryApiResources(Config.GetApiResources());
            builder.AddInMemoryClients(Config.GetClients());

            builder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            //添加证书加密方式，执行该方法，会先判断tempkey.rsa证书文件是否存在，
            //如果不存在的话，就创建一个新的tempkey.rsa证书文件，如果存在的话，就使用此证书文件
            builder.AddDeveloperSigningCredential();
        }

        public static void AddMyAuthentication(this IServiceCollection services) 
        {
            services.AddAuthentication("Bearer")
              .AddIdentityServerAuthentication(options =>
              {
                  //options.Authority = "http://localhost:5000";//授权服务器地址
                  //options.RequireHttpsMetadata = false;//不需要https    
                  options.ApiName = "api1";

              });
        }
    }
}
