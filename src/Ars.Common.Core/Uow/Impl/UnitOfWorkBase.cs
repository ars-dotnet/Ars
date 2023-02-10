using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Options;
using Ars.Common.Tool;
using Ars.Common.Tool.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow.Impl
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        public string Id { get; private set; }

        public IUnitOfWork Outer { get; set; }

        public UnitOfWorkOptions Options { get; private set; }

        public bool IsDisposed { get; private set; }

        public event EventHandler Completed;

        public event EventHandler<ArsUnitOfWorfEventFailedArgs> Failed;

        public event EventHandler Disposed;

        /// <summary>
        /// Is <see cref="Complete"/> method called before?
        /// </summary>
        private bool _isCompleteCalledBefore;

        private Exception _exception;

        private bool _succeed;

        public void Begin(UnitOfWorkOptions options)
        {
            Options = options;

            Id = Guid.NewGuid().ToString("N");

            BeginUow();
        }

        protected virtual void BeginUow() 
        {

        }

        protected abstract Task CompleteUowAsync();

        protected abstract void DisposeUow();

        public async Task CompleteAsync()
        {
            PreventMultipleComplete();

            try
            {
                await CompleteUowAsync();
                _succeed = true;
                CompletedHandler();
            }
            catch (Exception ex) 
            {
                _exception = ex;
            }
        }

        protected virtual void CompletedHandler() 
        {
            Completed.InvokeSafely(this);
        }

        protected virtual void FailedHandler()
        {
            Failed.InvokeSafely(this,new ArsUnitOfWorfEventFailedArgs(_exception));
        }

        protected virtual void DisposedHandler()
        {
            Disposed.InvokeSafely(this);
        }

        public void Dispose()
        {
            if (!_isCompleteCalledBefore || IsDisposed) 
            {
                return;
            }

            IsDisposed = true;

            if (!_succeed)
                FailedHandler();

            DisposeUow();
            DisposedHandler();
        }

        public abstract Task SaveChangesAsync();

        private void PreventMultipleComplete() 
        {
            if (_isCompleteCalledBefore)
                throw new ArsException("Complete is called before!");

            _isCompleteCalledBefore = true;
        }
    }
}
