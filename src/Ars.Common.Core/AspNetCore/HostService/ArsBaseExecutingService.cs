

using Ars.Commom.Tool.Extension;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Ars.Common.Core.AspNetCore.HostService
{
    public abstract class ArsBaseExecutingService : IDisposable
    {
        protected abstract TimeSpan DueTime { get; }

        protected abstract TimeSpan Period { get; }

        protected virtual TimeSpan WaitTime => TimeSpan.FromSeconds(10);

        private Timer? _timer;
        private CancellationTokenSource? _cancellationTokenSource;
        protected readonly ILogger _logger;
        private SemaphoreSlim _semaphoreSlim;
        protected ArsBaseExecutingService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _semaphoreSlim = new SemaphoreSlim(1,1);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            CancellationTokenSource tokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,_cancellationTokenSource.Token); 
            _timer = new Timer(CallBack, tokenSource, DueTime, Period);

            _logger.LogInformation("{0} executing service started",GetType().Name);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();

            _logger.LogInformation("{0} executing service stoped", GetType().Name);

            return StopExecutedAsync(cancellationToken);
        }

        protected async void CallBack(object? state)
        {
            if (!(state is CancellationTokenSource source) || source.Token.IsCancellationRequested)
                return;

            try
            {
                await _semaphoreSlim.LockAsync(WaitTime,source.Token, () => ExecutingAsync(source.Token));
            }
            catch (Exception e) 
            {
                _logger.LogError("{0} executing service failed,exception:{1}", GetType().Name,e);

                await FailedExecutingAsync(e, source.Token);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        protected abstract Task ExecutingAsync(CancellationToken cancellationToken);

        protected virtual Task StopExecutedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual Task FailedExecutingAsync(Exception e,CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
