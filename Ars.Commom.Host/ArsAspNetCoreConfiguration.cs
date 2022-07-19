using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Uow.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Host
{
    internal class ArsAspNetCoreConfiguration : IArsAspNetCoreConfiguration
    {
        public ArsAspNetCoreConfiguration()
        {
            unitOfWorkAttribute = new UnitOfWorkAttribute();
        }

        public UnitOfWorkAttribute unitOfWorkAttribute { get; private set; }
    }
}
