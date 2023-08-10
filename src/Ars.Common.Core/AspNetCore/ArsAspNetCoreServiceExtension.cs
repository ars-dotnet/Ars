using Ars.Commom.Core;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    internal class ArsAspNetCoreServiceExtension : IArsServiceExtension
    {
        public void AddService(IArsWebApplicationBuilder arsServiceBuilder)
        {
            arsServiceBuilder.AddAseNetCore();
        }
    }
}
