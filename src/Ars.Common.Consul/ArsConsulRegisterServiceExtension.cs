using Ars.Commom.Core;
using Ars.Common.Consul.Extension;
using Ars.Common.Consul.Option;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul
{
    public class ArsConsulRegisterServiceExtension : IArsServiceExtension
    {
        public void AddService(IArsWebApplicationBuilder arsServiceBuilder, IArsConfiguration? arsConfiguration = null)
        {
            arsServiceBuilder.AddArsConsulRegisterServer(arsConfiguration);
        }
    }
}
