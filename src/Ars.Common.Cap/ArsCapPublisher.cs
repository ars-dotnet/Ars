using Ars.Common.Core.IDependency;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Cap
{
    internal class ArsCapPublisher : IArsCapPublisher
    {
        private readonly ICapPublisher _capPublisher;
        private readonly ILogger _logger;
        public ArsCapPublisher(ICapPublisher capPublisher,ILoggerFactory loggerFactory)
        {
            _capPublisher = capPublisher;
            _logger = loggerFactory.CreateLogger<ArsCapPublisher>();
        }

        public IServiceProvider ServiceProvider => _capPublisher.ServiceProvider;

        public AsyncLocal<ICapTransaction> Transaction => _capPublisher.Transaction;

        public void Publish<T>(string name, T? contentObj, string? callbackName = null)
        {
            _logger.LogInformation($"Ars Publish Begin Log【name:{name},contentObj:{contentObj},callbackName:{callbackName}】");

            _capPublisher.Publish(name, contentObj, callbackName);
        }

        public void Publish<T>(string name, T? contentObj, IDictionary<string, string?> headers)
        {
            _logger.LogInformation($"Ars Publish Begin Log【name:{name},contentObj:{contentObj},headers:{headers}】");

            _capPublisher.Publish<T>(name, contentObj, headers);
        }

        public Task PublishAsync<T>(string name, T? contentObj, string? callbackName = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Ars PublishAsync Begin Log【name:{name},contentObj:{contentObj},callbackName:{callbackName}】");

            return _capPublisher.PublishAsync<T>(name,contentObj,callbackName,cancellationToken);
        }

        public Task PublishAsync<T>(string name, T? contentObj, IDictionary<string, string?> headers, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Ars PublishAsync Begin Log【name:{name},contentObj:{contentObj},headers:{headers}】");

            return _capPublisher.PublishAsync<T>(name,contentObj,headers,cancellationToken);
        }

        public void PublishDelay<T>(TimeSpan delayTime, string name, T? contentObj, IDictionary<string, string?> headers)
        {
            _logger.LogInformation($"Ars PublishDelay Begin Log【delayTime:{delayTime},name:{name},contentObj:{contentObj},headers:{headers}】");

            _capPublisher.PublishDelay(delayTime, name, contentObj, headers);
        }

        public void PublishDelay<T>(TimeSpan delayTime, string name, T? contentObj, string? callbackName = null)
        {
            _logger.LogInformation($"Ars PublishDelay Begin Log【delayTime:{delayTime},name:{name},contentObj:{contentObj},callbackName:{callbackName}】");

            _capPublisher.PublishDelay(delayTime,name,contentObj,callbackName);
        }

        public Task PublishDelayAsync<T>(TimeSpan delayTime, string name, T? contentObj, IDictionary<string, string?> headers, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Ars PublishDelayAsync Begin Log【delayTime:{delayTime},name:{name},contentObj:{contentObj},headers:{headers}】");

            return _capPublisher.PublishDelayAsync(delayTime, name, contentObj, headers);
        }

        public Task PublishDelayAsync<T>(TimeSpan delayTime, string name, T? contentObj, string? callbackName = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Ars PublishDelayAsync Begin Log【delayTime:{delayTime},name:{name},contentObj:{contentObj},callbackName:{callbackName}】");

            return _capPublisher.PublishDelayAsync(delayTime, name, contentObj, callbackName, cancellationToken);
        }
    }
}
