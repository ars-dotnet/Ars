using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace MyIdentittServer4.Configs
{
    public class Config
    {
        #region 定义资源
        // 身份信息授权资源
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        //API访问授权资源
        public static IEnumerable<ApiResource> GetApiResources() =>
            new ApiResource[]
            {
                //标识名称  显示名称(自定义)
                new ApiResource(OAuthConfig.ApiName,"myIds4")
                {
                    UserClaims =  { ClaimTypes.Name, JwtClaimTypes.Name },
                    ApiSecrets = new List<Secret>()
                    {
                        new Secret(OAuthConfig.Secret.Sha256())
                    }
                }
            };
        #endregion

        #region 定义客户端Client
        /// <summary>
        /// 4种模式 客户端模式(ClientCredentials) 密码模式(ResourceOwnerPassword)  隐藏模式(Implicit)  授权码模式(Code)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients() =>
            new Client[]
            {
                 new Client
                {
                    ClientId = OAuthConfig.ClientId,//客户端的唯一ID
                    ClientName = "密码模式",//客户端显示名称
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = new []{ new Secret(OAuthConfig.Secret.Sha256()) },//客户端加密方式
                    RequireConsent = false, //如果不需要显示否同意授权 页面 这里就设置为false  
                    AllowAccessTokensViaBrowser = true,//控制是否通过浏览器传输此客户端的访问令牌
                    AccessTokenLifetime = OAuthConfig.ExpireIn, //过期秒数
                    //登录成功跳转地址
                    RedirectUris =
                    {
                        "http://localhost:4137/oauth2-redirect.html",
                    },
                    //退出登录跳转地址
                    PostLogoutRedirectUris =
                    {
                        "http://www.baidu.com"
                    },
                    //跨域地址
                    AllowedCorsOrigins = OAuthConfig.CorUrls,
                    //配置授权范围，这里指定哪些API 受此方式保护
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        //如果要获取refresh_tokens ,必须在scopes中加上OfflineAccess
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        OAuthConfig.ApiName
                    },
                    AllowOfflineAccess=true// 主要刷新refresh_token,
                }
            };
        #endregion


        /// <summary>
        /// 测试的账号和密码
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser()
                {
                     SubjectId = "1",
                     Username = "admin",
                     Password = "123456"
                },
                new TestUser()
                {
                     SubjectId = "2",
                     Username = "test",
                     Password = "123456"
                }
            };
        }
    }
}
