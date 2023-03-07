using Ars.Common.Core.Configs;
using Ars.Common.Core.Localization.Extension;
using Microsoft.AspNetCore.Builder;

namespace Ars.Common.Core
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
