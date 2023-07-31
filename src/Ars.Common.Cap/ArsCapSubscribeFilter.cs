using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using DotNetCore.CAP.Filter;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ars.Common.Cap
{
    /// <summary>
    /// cpa消费端过滤器
    /// </summary>
    internal class ArsCapSubscribeFilter : ISubscribeFilter, ITransientDependency,IDisposable
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private IUnitOfWorkCompleteHandler? _scope;
        private readonly ILogger _logger;

        public ArsCapSubscribeFilter(IUnitOfWorkManager unitOfWorkManager,ILoggerFactory loggerFactory)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _logger = loggerFactory.CreateLogger<ArsCapSubscribeFilter>();
        }

        /// <summary>
        /// Called before the subscriber executes.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnSubscribeExecutingAsync(ExecutingContext context)
        {
            _logger.LogInformation($"Ars Subscribe Log【name:{context.ConsumerDescriptor.TopicName}】,contentObj:{JsonConvert.SerializeObject(context.DeliverMessage)}");

            _scope = _unitOfWorkManager.Begin(TransactionScopeOption.Required);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Called after the subscriber executes.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnSubscribeExecutedAsync(ExecutedContext context)
        {
            return _scope?.CompleteAsync() ?? Task.CompletedTask;
        }

        /// <summary>
        ///  Called after the subscriber has thrown an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnSubscribeExceptionAsync(ExceptionContext context)
        {
            _logger.LogError($"Ars Subscribe Err Log topic:{context.ConsumerDescriptor.TopicName} and err:{context.Exception}");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
