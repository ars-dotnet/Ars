using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.Core.Uow.Options
{
    public class UnitOfWorkOptions
    {
        /// <summary>
        /// Scope option.
        /// </summary>
        public TransactionScopeOption? Scope { get; set; }

        /// <summary>
        /// Is this UOW transactional?
        /// Uses default value if not supplied.
        /// </summary>
        public bool? IsTransactional { get; set; }

        /// <summary>
        /// Timeout of UOW As milliseconds.
        /// Uses default value if not supplied.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this UOW is transactional, this option indicated the isolation level of the transaction.
        /// Uses default value if not supplied.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        internal void FillDefaultsForNonProvidedOptions(IUnitOfWorkDefaultConfiguration configuration)
        {
            if (Scope == null)
            {
                Scope = configuration.Scope;
            }

            if (IsTransactional == null)
            {
                IsTransactional = configuration.IsTransactional;
            }

            if (Timeout == null && configuration.TimeOut.HasValue)
            {
                Timeout = configuration.TimeOut.Value;
            }

            if (IsolationLevel == null && configuration.IsolationLevel.HasValue) 
            {
                IsolationLevel = configuration.IsolationLevel.Value;
            }
        }

    }
}
