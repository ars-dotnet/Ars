using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Localization.options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Localization.Extension
{
    public static class IArsConfigurationExtension
    {
        public static void AddArsLocalization(this IArsConfiguration arsConfiguration) 
        {
            arsConfiguration.AddArsServiceExtension(new ArsLocalizationServiceExtension());
        }
    }
}
