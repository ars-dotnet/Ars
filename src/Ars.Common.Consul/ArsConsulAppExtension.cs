using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul
{
    internal class ArsConsulAppExtension : IArsAppExtension
    {
        public void UseApplication(IApplicationBuilder builder, IArsConfiguration configuration)
        {
            if (null != configuration.ArsConsulRegisterConfiguration)
            {
                builder.UseArsConsul(configuration.ArsConsulRegisterConfiguration);
            }
        }
    }
}
