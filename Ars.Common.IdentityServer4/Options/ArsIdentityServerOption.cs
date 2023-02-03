using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Configs;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Ars.Common.Core.Configs.IArsIdentityServerConfiguration;
using static IdentityServer4.IdentityServerConstants;

namespace Ars.Common.IdentityServer4.options
{
    public class ArsIdentityServerOption : IArsIdentityServerConfiguration
    {
        public IEnumerable<ArsApiResource> ArsApiResources { get; set; }
            //= new[]
            //{
            //    new ArsApiResource()
            //    {
            //        Name = "defaultApi",
            //        DisplayName = "my default api" ,
            //        UserClaims = new [] { ArsClaimTypes.Role,ArsClaimTypes.TenantId,ArsClaimTypes.UserId },
            //        Scopes = new []{ "defaultApi-scope", StandardScopes.OfflineAccess }
            //    }
            //};

        public IEnumerable<ArsApiClient> ArsClients { get; set; }
            //= new[]
            //{
            //    new ArsApiClient
            //    {
            //        AppKey = "default-Key",
            //        AppSecret = "default-Secret",
            //        AccessTokenLifetime = 99900,
            //        AllowedScopes = new [] { "defaultApi-scope", StandardScopes.OfflineAccess }
            //    }
            //};

        public IEnumerable<string> ArsApiScopes { get; set; }
            //= new[]
            //{
            //    "defaultApi-scope",
            //    StandardScopes.OfflineAccess
            //};

        public string CertPath { get; set; }

        public string Password { get; set; }
    }
}
