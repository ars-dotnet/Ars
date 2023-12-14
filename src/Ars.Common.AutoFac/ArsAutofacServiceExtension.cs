using Ars.Commom.Core;
using Ars.Common.AutoFac.Dependency;
using Ars.Common.AutoFac.Extension;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Ars.Common.AutoFac.RegisterProvider;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ars.Common.AutoFac
{
    internal class ArsAutofacServiceExtension : IArsServiceExtension
    {
        private readonly Action<PropertyAutowiredOption>? _autowiredAction;
        public ArsAutofacServiceExtension(Action<PropertyAutowiredOption>? action)
        {
            _autowiredAction = action;
        }

        public void AddService(IArsWebApplicationBuilder arsServiceBuilder)
        {
            arsServiceBuilder.AddAutofac(_autowiredAction);
        }
    }
}
