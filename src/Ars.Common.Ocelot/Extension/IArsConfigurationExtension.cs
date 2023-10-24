using Ars.Common.Core.Configs;
using Ocelot.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot.Extension
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsOcelot(this IArsConfiguration configuration, Action<IOcelotBuilder>? builderOption = null) 
        {
            configuration.AddArsServiceExtension(new ArsOcelotServiceExtension(builderOption));

            return configuration;
        }
    }
}
