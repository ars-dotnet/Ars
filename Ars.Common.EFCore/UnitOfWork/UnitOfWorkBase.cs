using Ars.Common.EFCore.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.UnitOfWork
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        public UnitOfWorkOptions Options { get; private set; }

        public bool IsDisposed { get; private set; }

        public string Id => throw new NotImplementedException();

        public event EventHandler Completed;

        public event EventHandler<ArsUnitOfWorfEventFailedArgs> Failed;

        public event EventHandler Disposed;

        public void Begin(UnitOfWorkOptions options)
        {
            throw new NotImplementedException();
        }

        public Task CompleteAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
