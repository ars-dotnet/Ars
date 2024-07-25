using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Options
{
    public class ArsIdentityClientConfiguration : IArsIdentityClientConfiguration
    {
        public string Authority { get; set; }

        public string ApiName { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public string? CertificatePath { get; set; }

        public string? CertificatePassWord { get; set; }

        /// <summary>
        /// token校验偏移时间多少秒
        /// </summary>
        public int? JwtValidationClockSkew { get; set; }
    }
}
