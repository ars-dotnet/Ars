using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow.DefaultImpl
{
    internal class DefaultUnitOfWork : UnitOfWorkBase,ITransientDependency
    {
        public override Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task CompleteUowAsync()
        {
            return Task.CompletedTask;
        }

        protected override void DisposeUow()
        {
            return;
        }
    }
}
