using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.Core.Uow
{
    public interface IUnitOfWorkDefaultConfiguration
    {
        public TransactionScopeOption Scope { get; set; }

        public bool IsTransactional { get; set; }

        public TimeSpan? TimeOut { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }
    }
}
