using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IArsLocalizationConfiguration
    {
        string ResourcesPath { get; set; }

        bool IsAddViewLocalization { get; set; }

        bool IsAddDataAnnotationsLocalization { get; set; }

        IEnumerable<string> Cultures { get; set; }

        string DefaultRequestCulture { get; set; }
    }
}
