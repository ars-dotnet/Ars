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
        public static void AddArsAspNetCore(this IArsConfiguration arsConfiguration)
        {
            arsConfiguration.AddArsServiceExtension(new ArsAspNetCoreServiceExtension());
        }
    }
}
