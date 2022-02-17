using Ars.Common.AutoFac.Helper;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Extension
{
    internal static class AutofacExtensions
    {
        internal static ILifetimeScope EnableAutowired(this ILifetimeScope lifetimeScope) 
        {
            AutofacPropertyAutowiredHelper.UseAutowired(lifetimeScope);

            return lifetimeScope;
        }

        
    }
}
