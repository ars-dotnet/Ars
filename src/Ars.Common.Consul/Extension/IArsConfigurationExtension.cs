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
        public static void AddArsConsulRegisterServer(this IArsConfiguration configuration) 
        {
            configuration.AddArsServiceExtension(new ArsConsulRegisterServiceExtension());
        }

        public static void AddArsConsulDiscoverClient(this IArsConfiguration configuration)
        {
            configuration.AddArsServiceExtension(new ArsConsulDiscoverServiceExtension());
        }
    }
}
