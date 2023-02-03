using Ars.Common.Core.Configs;
using Ars.Common.Core.Uow.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.Core.Uow.Impl
{
    internal class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IUnitOfWorkDefaultConfiguration _unitOfWorkDefaultConfiguration;
        private readonly IUnitOfWork _unitOfWork;
        public UnitOfWorkManager(ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkDefaultConfiguration unitOfWorkDefaultConfiguration,
            IUnitOfWork unitOfWork)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkDefaultConfiguration = unitOfWorkDefaultConfiguration;
            _unitOfWork = unitOfWork;
        }

        public IActiveUnitOfWork Current => _unitOfWorkProvider.Current;

        public IUnitOfWorkCompleteHandler Begin()
        {
            return Begin(new UnitOfWorkOptions());
        }

        public IUnitOfWorkCompleteHandler Begin(TransactionScopeOption scope)
        {
            return Begin(new UnitOfWorkOptions() { Scope = scope});
        }

        public IUnitOfWorkCompleteHandler Begin(UnitOfWorkOptions options)
        {
            options.FillDefaultsForNonProvidedOptions(_unitOfWorkDefaultConfiguration);

            var outerUow = _unitOfWorkProvider.Current;

            if (options.Scope == TransactionScopeOption.Required && outerUow != null)
            {
                return outerUow.Options?.Scope == TransactionScopeOption.Suppress
                    ? new InnerSuppressUnitOfWorkCompleteHandle(outerUow)
                    : new InnerUnitOfWorkCompleteHandle();
            }

            _unitOfWork.Completed += (sender, args) =>
            {
                _unitOfWorkProvider.Current = null;
            };

            _unitOfWork.Failed += (sender, args) =>
            {
                _unitOfWorkProvider.Current = null;
            };

            _unitOfWork.Disposed += (sender, args) =>
            {
                _unitOfWork.Dispose();
            };

            _unitOfWork.Begin(options);
            _unitOfWorkProvider.Current = _unitOfWork;

            return _unitOfWork;
        }
    }
}
