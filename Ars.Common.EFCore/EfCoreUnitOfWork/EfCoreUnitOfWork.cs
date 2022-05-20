using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.EfCoreUnitOfWork
{
    internal class EfCoreUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        protected override Task CompleteUowAsync()
        {
            throw new NotImplementedException();
        }

        protected override void DisposeUow()
        {
            throw new NotImplementedException();
        }
    }
}
