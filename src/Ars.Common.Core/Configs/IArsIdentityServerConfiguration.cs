using Ars.Common.Tool.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IArsIdentityServerConfiguration : IArsCertificateConfiguration
    {
        IEnumerable<ArsApiResource> ArsApiResources { get; set; }

        IEnumerable<ArsApiClient> ArsClients { get; set; }

        IEnumerable<string> ArsApiScopes { get; set; }

        bool UseTestUsers { get; set; }

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

            public bool AllowOfflineAccess { get; set; } = true;//如果要获取refresh_tokens ,必须把AllowOfflineAccess设置为true

            /// <summary>字符串逗号分割的API Resource资源</summary>
            public ICollection<string> AllowedScopes { get; set; }

            /// <summary>字符串逗号分割的客户端授权方式</summary>
            public ICollection<string> GrantType { get; set; }

            public ICollection<string> RedirectUris { get; set; }

            public ICollection<string> PostLogoutRedirectUris { get; set; }

            public ICollection<string> AllowedCorsOrigins { get; set; }
        }
    }
}
