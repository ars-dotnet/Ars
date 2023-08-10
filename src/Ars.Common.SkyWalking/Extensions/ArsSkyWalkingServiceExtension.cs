using Ars.Commom.Core;
using Ars.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SkyWalking.Extensions
{
    internal class ArsSkyWalkingServiceExtension : IArsServiceExtension
    {
        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsSkyApm();
        }
    }
}
