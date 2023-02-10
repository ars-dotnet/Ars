using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow
{
    public interface ICurrentUnitOfWorkProvider : ITransientDependency
    {
        /// <summary>
        /// Gets/sets current <see cref="IUnitOfWork"/>.
        /// Setting to null returns back to outer unit of work where possible.
        /// </summary>
        IUnitOfWork Current { get; set; }
    }
}
