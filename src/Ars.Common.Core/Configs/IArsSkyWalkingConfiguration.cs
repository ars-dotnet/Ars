using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IArsSkyWalkingConfiguration
    {
        string ASPNETCORE_HOSTINGSTARTUPASSEMBLIES { get; set; }

        string SKYWALKING__SERVICENAME { get; set; }
    }
}
