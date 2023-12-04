using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core
{
    public interface IArsServiceExtension
    {
        public void AddService(IArsWebApplicationBuilder applicationBuilder, IArsConfiguration? arsConfiguration = null);
    }
}
