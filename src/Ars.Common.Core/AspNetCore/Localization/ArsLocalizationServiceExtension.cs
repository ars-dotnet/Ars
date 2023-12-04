using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Localization.Extension;

namespace Ars.Common.Core
{
    internal class ArsLocalizationServiceExtension : IArsServiceExtension
    {
        public void AddService(IArsWebApplicationBuilder services, IArsConfiguration? arsConfiguration = null)
        {
            services.AddArsLocalization(arsConfiguration);
        }
    }
}
