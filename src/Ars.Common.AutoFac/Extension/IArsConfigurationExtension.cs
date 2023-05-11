using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Extension
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsAutofac(this IArsConfiguration configuration) 
        {
            configuration.AddArsServiceExtension(new ArsAutofacServiceExtension(null));

            return configuration;
        }
    }
}
