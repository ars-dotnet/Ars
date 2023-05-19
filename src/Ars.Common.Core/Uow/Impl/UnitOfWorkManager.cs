using Ars.Common.Core.Configs;
using Ars.Common.Core.Uow.Options;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _serviceProvider;
        public UnitOfWorkManager(ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkDefaultConfiguration unitOfWorkDefaultConfiguration,
            IServiceProvider serviceProvider)
        {
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkDefaultConfiguration = unitOfWorkDefaultConfiguration;
            _serviceProvider = serviceProvider;
        }

        public IActiveUnitOfWork Current => _unitOfWorkProvider.Current!;

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

            //required时，存在环境事务，则分两种情况
            //1.环境事务如果是Suppress，则返回InnerSuppressUnitOfWorkCompleteHandle实例，此实例直接SaveChangesAsync
            //2.环境事务如果不是Suppress，则返回InnerUnitOfWorkCompleteHandle实例，此实例没有保存事务操作
            ///如果要保留代码部分执行的操作，并且不希望在操作失败时中止环境事务，则 Suppress 非常有用
            if (options.Scope == TransactionScopeOption.Required && outerUow != null)
            {
                return outerUow.Options?.Scope == TransactionScopeOption.Suppress
                    ? new InnerSuppressUnitOfWorkCompleteHandle(outerUow)
                    : new InnerUnitOfWorkCompleteHandle();
            }

            var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

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
