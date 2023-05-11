using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.Extension
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsConsulRegisterServer(this IArsConfiguration configuration) 
        {
            configuration.AddArsServiceExtension(new ArsConsulRegisterServiceExtension());

            return configuration;
        }

        public static IArsConfiguration AddArsConsulDiscoverClient(this IArsConfiguration configuration)
        {
            configuration.AddArsServiceExtension(new ArsConsulDiscoverServiceExtension());

            return configuration;
        }
    }
}
