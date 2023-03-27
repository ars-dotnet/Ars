using Ars.Commom.Core;
using Ars.Common.Core.Localization.Extension;

namespace Ars.Common.Core
{
    internal class ArsLocalizationServiceExtension : IArsServiceExtension
    {
        public void AddService(IArsServiceBuilder services)
        {
            services.AddArsLocalization();
        }
    }
}
