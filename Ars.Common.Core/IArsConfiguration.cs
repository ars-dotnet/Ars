using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core
{
    public interface IArsConfiguration
    {
        public IUnitOfWorkDefaultConfiguration UnitOfWorkDefaultConfiguration { get; }
    }
}
