using Ars.Common.Core.Configs;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core
{
    public interface IArsAppExtension
    {
        void UseApplication(IApplicationBuilder builder, IArsConfiguration configuration);
    }
}
