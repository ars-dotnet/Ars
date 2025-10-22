using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Ars.Common.Core.Configs.IArsIdentityServerConfiguration;

namespace Ars.Common.IdentityServer4.options
{
    public class ArsIdentityServerConfiguration : IArsIdentityServerConfiguration
    {
        public IEnumerable<ArsApiResource> ArsApiResources { get; set; }

        public IEnumerable<ArsApiClient> ArsClients { get; set; }

        public IEnumerable<string> ArsApiScopes { get; set; }

        public bool UseTestUsers { get; set; }

        public string? CertificatePath { get; set; }

        public string? CertificatePassWord { get; set; }
    }
}
