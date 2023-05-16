using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SkyWalking
{
    internal class ArsSkyWalkingConfiguration : IArsSkyWalkingConfiguration
    {
        public string ASPNETCORE_HOSTINGSTARTUPASSEMBLIES { get; set; }

        public string SKYWALKING__SERVICENAME { get; set; }
    }
}
