using Ars.Common.Core.AspNetCore.HostService;

namespace ArsWebApiService.StartupHostServices
{
    //public class Host2Service : ArsHostStartupExecutingService
    //{
    //    private int i;
    //    public Host2Service(ILoggerFactory loggerFactory) : base(loggerFactory)
    //    {

    //    }

    //    protected override TimeSpan DueTime => TimeSpan.FromSeconds(10);

    //    /// <summary>
    //    /// 只调用一次
    //    /// </summary>
    //    protected override TimeSpan Period => Timeout.InfiniteTimeSpan;

    //    protected override Task ExecutingAsync(CancellationToken cancellationToken)
    //    {
    //        Interlocked.Increment(ref i);
    //        _logger.LogInformation($"{nameof(Host2Service)}.{nameof(ExecutingAsync)}.{i}");

    //        return Task.CompletedTask;
    //    }

    //    protected override Task StopExecutedAsync(CancellationToken cancellationToken)
    //    {
    //        _logger.LogInformation($"{nameof(Host2Service)}.{StopExecutedAsync}.{i}");

    //        return Task.CompletedTask;
    //    }

    //    protected override Task FailedExecutingAsync(Exception e, CancellationToken cancellationToken)
    //    {
    //        _logger.LogError($"{nameof(Host2Service)}.{FailedExecutingAsync}.{i}");

    //        return Task.CompletedTask;
    //    }
    //}
}
