using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.Extensions
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsAspNetCore(this IArsConfiguration arsConfiguration)
        {
            return arsConfiguration.AddArsServiceExtension(new ArsAspNetCoreServiceExtension());
        }
    }
}
