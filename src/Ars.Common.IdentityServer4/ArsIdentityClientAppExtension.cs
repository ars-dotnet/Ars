using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.IdentityServer4.Extension;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4
{
    internal class ArsIdentityClientAppExtension : IArsAppExtension
    {
        public void UseApplication(IApplicationBuilder builder, IArsConfiguration configuration)
        {
            if (null != configuration.ArsIdentityClientConfiguration)
            {
                builder.UseArsIdentityClient();
            }
        }
    }
}
