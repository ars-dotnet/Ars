using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow
{
    public interface IUnitOfWorkCompleteHandler : IDisposable
    {
        /// <summary>
        /// Completes this unit of work.
        /// It saves all changes and commit transaction if exists.
        /// </summary>
        Task CompleteAsync();
    }
}
