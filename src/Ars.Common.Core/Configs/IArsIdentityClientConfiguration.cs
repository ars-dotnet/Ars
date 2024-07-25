using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IArsIdentityClientConfiguration : IArsCertificateConfiguration
    {
        string Authority { get; }

        string ApiName { get; }

        bool RequireHttpsMetadata { get; }

        /// <summary>
        /// token校验偏移时间多少秒
        /// </summary>
        int? JwtValidationClockSkew { get; }
    }
}
