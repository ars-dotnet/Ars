using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Localization.options
{
    public class ArsLocalizationOption
    {
        public string ResourcesPath { get; set; } = "Resources";

        public bool IsAddViewLocalization { get; set; } = false;

        public bool IsAddDataAnnotationsLocalization { get; set; } = true;

        public IEnumerable<string> Cultures = new[] { "en-US", "zh-Hans" };

        public string DefaultRequestCulture = "en-US";
    }
}
