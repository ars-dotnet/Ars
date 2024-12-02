using Ars.Common.Core.AspNetCore.HostService;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using System.Transactions;

namespace ArsWebApiService.Services
{
    public class HostService : ArsBaseHostStartupExecutingService
    {
        public HostService(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override TimeSpan DueTime => TimeSpan.FromSeconds(10);

        protected override TimeSpan Period => TimeSpan.FromSeconds(10);

        [Autowired]
        protected IServiceScopeFactory ServiceScopeFactory { get; set; }

        protected override async Task ExecutingAsync(CancellationToken cancellationToken)
        {
            //var provider = ServiceScopeFactory.CreateScope().ServiceProvider;

            //var manager = provider.GetRequiredService<IUnitOfWorkManager>();

            //using var scope = manager.Begin(TransactionScopeOption.RequiresNew);

            //var service = provider.GetRequiredService<IMService>();

            //var data = await service.GetAsync();

            //await scope.CompleteAsync();
        }
    }
}
