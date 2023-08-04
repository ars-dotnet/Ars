using Ars.Common.Core.AspNetCore.HostService;

namespace ArsWebApiService.StartupHostServices
{
    //public class Host1Service : ArsHostStartupExecutingService
    //{
    //    private int i;
    //    public Host1Service(ILoggerFactory loggerFactory) : base(loggerFactory)
    //    {

    //    }

    //    protected override TimeSpan DueTime => TimeSpan.FromSeconds(10);

    //    protected override TimeSpan Period => TimeSpan.FromSeconds(30);

    //    protected override Task ExecutingAsync(CancellationToken cancellationToken)
    //    {
    //        Interlocked.Increment(ref i);
    //        _logger.LogInformation($"{nameof(Host1Service)}.{nameof(ExecutingAsync)}.{i}");

    //        return Task.CompletedTask;
    //    }

    //    protected override Task StopExecutedAsync(CancellationToken cancellationToken)
    //    {
    //        _logger.LogInformation($"{nameof(Host1Service)}.{StopExecutedAsync}.{i}");

    //        return Task.CompletedTask;
    //    }

    //    protected override Task FailedExecutingAsync(Exception e, CancellationToken cancellationToken)
    //    {
    //        _logger.LogError($"{nameof(Host1Service)}.{FailedExecutingAsync}.{i}");

    //        return Task.CompletedTask;
    //    }
    //}
}
