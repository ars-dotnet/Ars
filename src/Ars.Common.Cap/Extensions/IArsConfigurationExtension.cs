using Ars.Common.Core.Configs;
using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Cap.Extensions
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsCap(this IArsConfiguration arsConfiguration, Action<CapOptions> setupAction)
        {
            return arsConfiguration.AddArsServiceExtension(new ArsCapServiceExtension(setupAction));
        }
    }
}
