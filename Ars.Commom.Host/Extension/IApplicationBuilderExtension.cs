using Ars.Common.AutoFac.RegisterProvider;
using Ars.Common.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Host.Extension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsCore(this IApplicationBuilder applicationBuilder) 
        {
            
            return applicationBuilder;
        }
    }
}
