using Ars.Common.Core.Uow.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.Core.Uow.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class UnitOfWorkAttribute : Attribute
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

        /// <summary>
        /// Used to prevent starting a unit of work for the method.
        /// If there is already a started unit of work, this property is ignored.
        /// Default: false.
        /// </summary>
        public bool IsDisabled { get; set; }

        public UnitOfWorkAttribute()
        {

        }

        internal UnitOfWorkOptions CreateOption()
        {
            return new UnitOfWorkOptions()
            {
                Scope = Scope,
                IsTransactional = IsTransactional,
                Timeout = Timeout,
                IsolationLevel = IsolationLevel,
            };
        }
    }
}
