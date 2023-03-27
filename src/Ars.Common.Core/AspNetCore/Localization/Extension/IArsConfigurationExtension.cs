using Ars.Common.Core.Configs;

namespace Ars.Common.Core.Localization.Extension
{
    public static class IArsConfigurationExtension
    {
        public static void AddArsLocalization(this IArsConfiguration arsConfiguration) 
        {
            arsConfiguration.AddArsServiceExtension(new ArsLocalizationServiceExtension());
        }
    }
}
