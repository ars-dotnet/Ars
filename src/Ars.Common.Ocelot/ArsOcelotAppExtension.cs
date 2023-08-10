using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Ocelot.Extension;
using Microsoft.AspNetCore.Builder;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot
{
    internal class ArsOcelotAppExtension : IArsAppExtension
    {
        public void UseApplication(IApplicationBuilder builder, IArsConfiguration configuration)
        {
            if (null != configuration.ArsOcelotConfiguration) 
            {
                builder.UseArsOcelot();
            }
        }
    }
}
