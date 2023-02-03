using Ars.Commom.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core
{
    public interface IArsOptExtension
    {
        public void AddService(IArsServiceBuilder services);
    }
}
