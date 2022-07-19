using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore
{
    public interface IArsAspNetCoreConfiguration
    {
        UnitOfWorkAttribute unitOfWorkAttribute { get; }
    }
}
