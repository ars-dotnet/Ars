using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Localization.options
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
