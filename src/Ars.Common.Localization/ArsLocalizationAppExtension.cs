using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Localization.Extension;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Localization
{
    internal class ArsLocalizationAppExtension : IArsAppExtension
    {
        public void UseApplication(IApplicationBuilder builder, IArsConfiguration configuration)
        {
            if(null != configuration.ArsLocalizationConfiguration)
            {
                builder.UseArsLocalization();
            }
        }
    }
}
