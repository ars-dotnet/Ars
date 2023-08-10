using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Ocelot.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot
{
    internal class ArsOcelotServiceExtension : IArsServiceExtension
    {
        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsOcelot();
        }
    }
}
