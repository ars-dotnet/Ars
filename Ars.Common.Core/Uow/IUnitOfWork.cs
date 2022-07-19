using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow
{
    public interface IUnitOfWork : IActiveUnitOfWork,IUnitOfWorkCompleteHandler,ITransientDependency
    {
        /// <summary>
        /// Unique id of this UOW.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Reference to the outer UOW if exists.
        /// </summary>
        IUnitOfWork Outer { get; set; }

        /// <summary>
        /// Begins the unit of work with given options.
        /// </summary>
        /// <param name="options">Unit of work options</param>
        void Begin(UnitOfWorkOptions options);
    }
}
