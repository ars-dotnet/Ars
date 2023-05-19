using Ars.Common.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow.Impl
{
    internal class InnerUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandler
    {
        private bool _isComplate;
        private bool _isDisposed;
        public virtual Task CompleteAsync()
        {
            _isComplate = true; return Task.CompletedTask; 
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (!_isComplate) 
            {
                if (HasException())
                    return;

                throw new ArsException("Did not call Complete method of a unit of work.");
            }
        }

        private bool HasException() 
        {
            try
            {
                return Marshal.GetExceptionCode() != 0;
            }
            catch (Exception) 
            {
                return false;
            }
        }
    }
}
