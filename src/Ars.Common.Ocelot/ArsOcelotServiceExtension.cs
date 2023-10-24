using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Ocelot.Extension;
using Ocelot.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Ocelot
{
    internal class ArsOcelotServiceExtension : IArsServiceExtension
    {
        private readonly Action<IOcelotBuilder>? _builderOption;
        public ArsOcelotServiceExtension(Action<IOcelotBuilder>? builderOption)
        {
            _builderOption = builderOption;
        }

        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsOcelot(_builderOption);
        }
    }
}
