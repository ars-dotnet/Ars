using Ars.Commom.Core;
using Ars.Common.Cap.Extensions;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Cap
{
    public class ArsCapServiceExtension : IArsServiceExtension
    {
        private readonly Action<CapOptions> _setupAction;
        public ArsCapServiceExtension(Action<CapOptions> setupAction)
        {
            _setupAction = setupAction;
        }

        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsCap(_setupAction);
        }
    }
}
