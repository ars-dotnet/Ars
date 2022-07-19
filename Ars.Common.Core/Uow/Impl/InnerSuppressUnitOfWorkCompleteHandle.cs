using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow.Impl
{
    internal class InnerSuppressUnitOfWorkCompleteHandle : InnerUnitOfWorkCompleteHandle
    {
        private readonly IUnitOfWork _unitOfWork;
        public InnerSuppressUnitOfWorkCompleteHandle(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task CompleteAsync()
        {
            await _unitOfWork.SaveChangesAsync();
            await base.CompleteAsync();
        }
    }
}
