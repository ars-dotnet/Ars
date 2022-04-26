using Ars.Common.Core.AspNetCore;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.options
{
    public class ArsIdentityServerOption
    {
        public IEnumerable<ArsApiResource> ArsApiResources { get; set; }
            = new[]
            {
                new ArsApiResource()
                { 
                    Name = "defaultApi",
                    DisplayName = "my default api" ,
                    UserClaims = new [] { ArsClaimTypes.Role,ArsClaimTypes.TenantId,ArsClaimTypes.UserId },
                    Scopes = new []{ "defaultApi-scope" } 
                }
            };

        public IEnumerable<ArsApiClient> ArsClients { get; set; }
            = new[]
            {
                new ArsApiClient
                {
                    AppKey = "default-Key",
                    AppSecret = "default-Secret",
                    AccessTokenLifetime = 99900,
                    AllowedScopes = new [] { "defaultApi-scope" }
                }
            };

        public IEnumerable<string> ArsApiScopes { get; set; }
            = new[]
            {
                "defaultApi-scope"
            };

        public class ArsApiResource 
        {
            public string Name { get; set; }

            public string DisplayName { get; set; }

            public ICollection<string> UserClaims { get; set; }

            public ICollection<string> Scopes { get; set; }
        }

        public class ArsApiClient 
        {
            public string AppKey { get; set; }

            public string AppSecret { get; set; }

            /// <summary>过期时间（秒）</summary>
            public int AccessTokenLifetime { get; set; }

            /// <summary>字符串逗号分割的API Resource资源</summary>
            public ICollection<string> AllowedScopes { get; set; }

            /// <summary>字符串逗号分割的客户端授权方式</summary>
            public ICollection<string> GrantType { get; set; } = GrantTypes.ResourceOwnerPassword;

            public ICollection<string> RedirectUris { get; set; }

            public ICollection<string> PostLogoutRedirectUris { get; set; }

            public ICollection<string> AllowedCorsOrigins { get; set; }
        }
    }
}
