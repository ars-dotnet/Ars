using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Uow;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Host
{
    internal class ArsConfiguration : IArsConfiguration
    {
        public IUnitOfWorkDefaultConfiguration UnitOfWorkDefaultConfiguration { get; }

        public ArsConfiguration(IUnitOfWorkDefaultConfiguration unitOfWorkDefaultConfiguration)
        {
            UnitOfWorkDefaultConfiguration = unitOfWorkDefaultConfiguration;
        }
    }
}
