using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.HostService
{
    public interface IArsManualExecutingService : IArsExecutingService, ISingletonDependency
    {

    }
}
