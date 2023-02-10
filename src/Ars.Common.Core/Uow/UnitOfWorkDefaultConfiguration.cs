using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.Core
{
    internal class UnitOfWorkDefaultConfiguration : IUnitOfWorkDefaultConfiguration
    {
        public TransactionScopeOption Scope { get; set; }

        public bool IsTransactional { get; set; }

        public TimeSpan? TimeOut { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }

        public UnitOfWorkDefaultConfiguration()
        {
            Scope = TransactionScopeOption.Required;
            IsTransactional = true;
        }
    }
}
