using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SkyWalking.Extensions
{
    public static class IArsConfigurationExtension
    {
        public static void AddArsSkyApm(this IArsConfiguration arsConfiguration) 
        {
            arsConfiguration.AddArsServiceExtension(new ArsSkyWalkingServiceExtension());
        }
    }
}
