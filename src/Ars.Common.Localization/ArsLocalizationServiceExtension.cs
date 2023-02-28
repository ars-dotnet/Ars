using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Localization.IServiceCollectionExtension;
using Ars.Common.Localization.options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Localization
{
    internal class ArsLocalizationServiceExtension : IArsServiceExtension
    {
        public void AddService(IArsServiceBuilder services)
        {
            services.AddArsLocalization();
        }
    }
}
