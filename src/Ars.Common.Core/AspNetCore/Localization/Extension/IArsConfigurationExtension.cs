using Ars.Common.Core.Configs;

namespace Ars.Common.Core.Localization.Extension
{
    public static class IArsConfigurationExtension
    {
        public static IArsConfiguration AddArsLocalization(this IArsConfiguration arsConfiguration) 
        {
            return arsConfiguration.AddArsServiceExtension(new ArsLocalizationServiceExtension());
        }
    }
}
