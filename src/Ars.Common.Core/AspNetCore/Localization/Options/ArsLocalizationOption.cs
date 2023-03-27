using Ars.Common.Core.Configs;

namespace Ars.Common.Core.Localization.Options
{
    public class ArsLocalizationConfiguration : IArsLocalizationConfiguration
    {
        public string ResourcesPath { get; set; } = "Resources";

        public bool IsAddViewLocalization { get; set; } = false;

        public bool IsAddDataAnnotationsLocalization { get; set; } = true;

        public IEnumerable<string> Cultures { get; set; }

        public string DefaultRequestCulture { get; set; } = "en-US";
    }
}
